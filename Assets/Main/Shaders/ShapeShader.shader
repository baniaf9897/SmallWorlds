Shader "Custom/ShapeShader"
{
    Properties
    {
         _LightColor("LightColor", Color) = (1,1,1,1)

    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            Cull Off
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
            float4 vertex   : SV_POSITION;
            fixed4 color : COLOR;
            half3 normal : TEXCOORD0;
            float4 diff : TEXCOORD1; // diffuse lighting color
        };

        struct MeshProperties {
            float4x4 mat;
            float4 color;
        };

        StructuredBuffer<MeshProperties> _Properties;
        float4 _LightColor;




        float4x4 rotateAlign(float3 v1, float3 v2)
        {
            float3 axis = cross(v1, v2);

            const float cosA = dot(v1, v2);
            const float k = 1.0f / (1.0f + cosA);

            return float4x4(
                (axis.x * axis.x * k) + cosA,
                (axis.y * axis.x * k) - axis.z,
                (axis.z * axis.x * k) + axis.y,
                0.0,
                (axis.x * axis.y * k) + axis.z,
                (axis.y * axis.y * k) + cosA,
                (axis.z * axis.y * k) - axis.x,
                0.0,
                (axis.x * axis.z * k) - axis.y,
                (axis.y * axis.z * k) + axis.x,
                (axis.z * axis.z * k) + cosA,
                0.0,
                0.0,
                0.0,
                0.0,
                1.0
                );
        }




        v2f vert(appdata_t i, uint instanceID: SV_InstanceID)
        {
            v2f o;

            float4 world_pos = mul(_Properties[instanceID].mat, i.vertex);//mul(UNITY_MATRIX_M, mul(_Properties[instanceID].mat, i.vertex));

            float4 view_pos = mul(UNITY_MATRIX_V, world_pos);// just for testing , works with Cull Off 
            o.vertex = mul(UNITY_MATRIX_P, view_pos);

            o.color = _Properties[instanceID].color;
            o.normal = UnityObjectToWorldNormal(i.normal);
 
            //standard diffuse lighting (lambert)
            half nl = max(0, dot(o.normal, _WorldSpaceLightPos0.xyz));
            o.diff = nl * _LightColor;

             return o;
         }

         fixed4 frag(v2f i, fixed facing : VFACE) : SV_Target
         {
             return i.color * i.diff;
         }
         ENDCG
     }
    }
}
