Shader "Custom/ShapeShader"
{
    Properties
    {

    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

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
        };

        struct MeshProperties {
            float4x4 mat;
            float4 color;
        };

        StructuredBuffer<MeshProperties> _Properties;


        v2f vert(appdata_t i, uint instanceID: SV_InstanceID)
        {
            v2f o;
            //float3 worldPos = mul(_Properties[instanceID].mat, i.vertex);
            //o.sphPos = mul( _Properties[instanceID].mat, float3(0.05, 0.05, 0.05));

            o.rayOrigin = _WorldSpaceCameraPos.xyz;


            // get object's world space pivot from the transform matrix
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


            o.sphPos = _Properties[instanceID].mat._m03_m13_m23 + mul(float3(0.05, 0.05, 0.05),rotMat);

            // transform into view space
          //  float3 viewSpacePivot = mul(UNITY_MATRIX_V, float4(worldSpacePivot, 1.0));
            // object space vertex position + view pivot = billboarded quad
          //  float3 viewSpacePos = i.vertex + viewSpacePivot;
            // calculate the object space ray dir from the view space position
          //  o.rayDir = mul(unity_WorldToObject,
           //     mul(UNITY_MATRIX_I_V, float4(viewSpacePos, 0.0))
           // );
            // apply projection matrix to get clip space position
            o.pos = UnityWorldToClipPos(worldPos);
            o.color = _Properties[instanceID].color;

            return o;


      
           




           /* float3 worldPos = mul(_Properties[instanceID].mat, i.vertex);
            o.sphPos = _Properties[instanceID].mat._m03_m13_m23 + float3(0.05,0.05,0.05);
            // calculate and world space ray direction and origin for interpolation
            o.rayDir = worldPos - _WorldSpaceCameraPos.xyz;
            o.rayOrigin = _WorldSpaceCameraPos.xyz;
            o.color = _Properties[instanceID].color;

            o.pos = UnityWorldToClipPos(worldPos);
            return o;*/








           /* float4 vertex_pos = mul(_Properties[instanceID].mat, i.vertex);//mul(UNITY_MATRIX_M, mul(_Properties[instanceID].mat, i.vertex));
           
            o.rayOrigin = _WorldSpaceCameraPos.xyz;

            float3 worldSpacePivot = _Properties[instanceID].mat._m03_m13_m23;// nity_ObjectToWorld._m03_m13_m23;
            o.sphPos = worldSpacePivot;

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
            float3 worldPos = mul(float3(vertex_pos.xy, 0.03), rotMat); //worldSpacePivot;
            // ray direction
            float3 worldRayDir = worldPos - _WorldSpaceCameraPos.xyz;
            o.rayDir = worldRayDir;// mul(unity_WorldToObject, float4(worldRayDir, 0.0));
            // clip space position output
            o.pos = UnityWorldToClipPos(worldPos);

            o.color = _Properties[instanceID].color;
            return o;*/




           /* float4 view_pos = mul(UNITY_MATRIX_V, world_pos);// just for testing , works with Cull Off 
            o.vertex = mul(UNITY_MATRIX_P, view_pos);
            o.worldPos = world_pos;
            o.color = _Properties[instanceID].color;
            o.normal = UnityObjectToWorldNormal(i.normal);
            */

            // billboard mesh towards camera
           /* float3 vpos = mul((float3x3)unity_ObjectToWorld, world_pos);
            float4 worldCoord = float4(unity_ObjectToWorld._m03, unity_ObjectToWorld._m13, unity_ObjectToWorld._m23, 1);
            float4 viewPos = mul(UNITY_MATRIX_V, worldCoord) + float4(vpos, 0);
            float4 outPos = mul(UNITY_MATRIX_P, viewPos);
            
            o.worldPos = outPos;
            o.vertex = outPos;//mul(UNITY_MATRIX_P, view_pos);
            //o.worldPos = world_pos;
            o.color = _Properties[instanceID].color;
            o.normal = UnityObjectToWorldNormal(i.normal);*/




 
            //standard diffuse lighting (lambert)
            //half nl = max(0, dot(o.normal, _WorldSpaceLightPos0.xyz));
            //o.diff = nl * _LightColor;

            // return o;
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
             
          /*  if (i.worldPos.y > 10 || i.worldPos.y < -10 || i.worldPos.x > 10 || i.worldPos.x < -10 || i.worldPos.z > 10 || i.worldPos.z < -10) {
                return i.color * i.diff * 0.1;
            }
            */




              

            //return i.color *  i.diff;



               // ray origin
                float3 rayOrigin = i.rayOrigin;

                // normalize ray vector
                float3 rayDir = normalize(i.rayDir);

                // sphere position
                float3 spherePos = i.sphPos;// unity_ObjectToWorld._m03_m13_m23;

                // ray box intersection
                float rayHit = sphIntersect(rayOrigin, rayDir, float4(spherePos, 0.03));

                // above function returns -1 if there's no intersection
                clip(rayHit);
               // if (rayHit == -1) {
                //    return half4(0.2, 0.2, 0.2, 1);
               // }
         
                // calculate world space position from ray, front hit ray length, and ray origin
                float3 worldPos = rayDir * rayHit + rayOrigin;

                // world space surface normal
                float3 worldNormal = normalize(worldPos - spherePos);

                // basic lighting
                half3 worldLightDir = _WorldSpaceLightPos0.xyz;
                half ndotl = saturate(dot(worldNormal, worldLightDir));
                half3 lighting = i.color * ndotl;

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
