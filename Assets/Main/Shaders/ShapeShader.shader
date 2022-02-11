Shader "Custom/ShapeShader"
{
    Properties
    {

    }
        SubShader
    {
        //Tags { "RenderType" = "Opaque" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

        // make fog work

        #include "UnityCG.cginc"

        struct appdata_t
        {
            float4 vertex   : POSITION;
            float4 color    : COLOR;
            float3 normal   : NORMAL;
        };

        struct v2f
        {
            float4 pos   : SV_POSITION;
            fixed4 color : COLOR;
            float3 rayDir : TEXCOORD0;
            float3 rayOrigin : TEXCOORD1;
            float3 sphPos : TEXCOORD2;
            float scale : TEXCOORD3;
        };

        struct MeshProperties {
            float4x4 mat;
            float4 color;
            float scale;
        };

        StructuredBuffer<MeshProperties> _Properties;


        v2f vert(appdata_t i, uint instanceID: SV_InstanceID)
        {
            v2f o;

            float3 worldSpacePivot = _Properties[instanceID].mat._m03_m13_m23 ;
            float3 worldSpacePivotToCamera = _WorldSpaceCameraPos.xyz - worldSpacePivot;

            float3 up = UNITY_MATRIX_I_V._m01_m11_m21;
            float3 forward = normalize(worldSpacePivotToCamera);
            float3 right = normalize(cross(forward, up));
            up = cross(right, forward);
            float3x3 rotMat = float3x3(right, up, forward);

            float3 worldPos = mul(float3(i.vertex.xy, 0.3), rotMat) + worldSpacePivot;
            float3 worldRayDir = worldPos - _WorldSpaceCameraPos.xyz;
            o.rayDir = mul(unity_WorldToObject, float4(worldRayDir, 0.0));           
            o.rayOrigin = _WorldSpaceCameraPos.xyz;

            o.sphPos = _Properties[instanceID].mat._m03_m13_m23 + mul(float3(0.05, 0.05, 0.05),rotMat);
            o.pos = UnityWorldToClipPos(worldPos);
            o.color = _Properties[instanceID].color;


            o.scale = _Properties[instanceID].scale;
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

         fixed4 frag(v2f i, out float outDepth : SV_Depth) : SV_Target
         {

             
            float3 rayOrigin = i.rayOrigin;
            float3 rayDir = normalize(i.rayDir);

            float3 spherePos = i.sphPos;

            float rayHit = sphIntersect(rayOrigin, rayDir, float4(spherePos, 0.02 * i.scale));

            clip(rayHit);
          
            float3 worldPos = rayDir * rayHit + rayOrigin;
            float3 worldNormal = normalize(worldPos - spherePos);

           

            // basic lighting
            half3 worldLightDir = _WorldSpaceLightPos0.xyz;
            half ndotl = saturate(dot(worldNormal, worldLightDir));
            half3 lighting = i.color * ndotl;



            float3 viewDirection = normalize(_WorldSpaceCameraPos - worldPos);
            float3 refl = normalize(reflect(-worldLightDir, worldNormal));
            float RdotV = max(0., dot(refl, viewDirection));
            fixed3 spec = pow(RdotV, 0.9) * float3(1,1,1) * ceil(ndotl) * float3(1, 1, 1) * 0.3;

            // output modified depth
            float4 clipPos = UnityWorldToClipPos(worldPos);
            outDepth = clipPos.z / clipPos.w;

            return half4(lighting + spec, i.color.w);
         }
         ENDCG
     }
    }
}
