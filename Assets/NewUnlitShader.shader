Shader "Basic Sphere Impostor"
{
    Properties
    {
     
        _LightColor("LightColor", Color) = (1,1,1,1)

    }
    SubShader
    {
        Tags { "RenderType" = "AlphaTest" "DisableBatching" = "True" }
        LOD 100

        Pass
        {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

        float4 _LightColor;

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 rayDir : TEXCOORD0;
                float3 rayOrigin : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;

                o.rayOrigin = _WorldSpaceCameraPos.xyz;

             
                float3 worldSpacePivot = float3(0, 0, 0);//unity_ObjectToWorld._m03_m13_m23;
                // offset between pivot and camera
                float3 worldSpacePivotToCamera = _WorldSpaceCameraPos.xyz - worldSpacePivot;
                // camera up vector
                // used as a somewhat arbitrary starting up orientation
                float3 up = UNITY_MATRIX_I_V._m01_m11_m21;
                // forward vector is the normalized offset
                // this it the direction from the pivot to the camera
                float3 forward = normalize(worldSpacePivotToCamera);
                // cross product gets a vector perpendicular to the input vectors
                float3 right = normalize(cross(forward, up));
                // another cross product ensures the up is perpendicular to both
                up = cross(right, forward);
                // construct the rotation matrix
                float3x3 rotMat = float3x3(right, up, forward);
                // the above rotate matrix is transposed, meaning the components are
                // in the wrong order, but we can work with that by swapping the
                // order of the matrix and vector in the mul()
                float3 worldPos = mul(float3(v.vertex.xy, 0.3), rotMat) + worldSpacePivot;
                // ray direction
                float3 worldRayDir = worldPos - _WorldSpaceCameraPos.xyz;
                o.rayDir = mul(unity_WorldToObject, float4(worldRayDir, 0.0));
                // clip space position output
                o.pos = UnityWorldToClipPos(worldPos);
                return o;
            }

            // https://www.iquilezles.org/www/articles/spherefunctions/spherefunctions.htm
            float sphIntersect(float3 ro, float3 rd, float4 sph)
            {
                float3 oc = ro - sph.xyz;
                float b = dot(oc, rd);
                float c = dot(oc, oc) - sph.w * sph.w;
                float h = b * b - c;
                if (h < 0.0) return -1.0;
                h = sqrt(h);
                return -b - h;
            }

            half3 _LightColor0;

            half4 frag(v2f i, out float outDepth : SV_Depth) : SV_Target
            {
                // ray origin
                float3 rayOrigin = i.rayOrigin;

                // normalize ray vector
                float3 rayDir = normalize(i.rayDir);

                // sphere position
                float3 spherePos = unity_ObjectToWorld._m03_m13_m23;

                // ray box intersection
                float rayHit = sphIntersect(rayOrigin, rayDir, float4(spherePos, 0.5));

                // above function returns -1 if there's no intersection
                clip(rayHit);

                // calculate world space position from ray, front hit ray length, and ray origin
                float3 worldPos = rayDir * rayHit + rayOrigin;

                // world space surface normal
                float3 worldNormal = normalize(worldPos - spherePos);

                // basic lighting
                half3 worldLightDir = _WorldSpaceLightPos0.xyz;
                half ndotl = saturate(dot(worldNormal, worldLightDir));
                half3 lighting = _LightColor * ndotl;

                // ambient lighting
             //   half3 ambient = ShadeSH9(float4(worldNormal, 1));
              //  lighting += ambient;    

                // output modified depth
                float4 clipPos = UnityWorldToClipPos(worldPos);
                outDepth = clipPos.z / clipPos.w;

                return half4(lighting, 1.0);
            }
            ENDCG
        }
    }
}