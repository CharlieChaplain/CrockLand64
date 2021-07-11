Shader "Custom/ShowVertColors_VL"
{
    SubShader
    {
        Pass
        {
            Lighting On

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos :SV_POSITION;
                float4 color :TEXCOORD0;
            };


            v2f vert(appdata_full v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);

                o.color = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), 1.0);
                o.color *= v.color;

                return o;
            }

            float4 frag(v2f i) : COLOR
            {
                return i.color;
            }

            ENDCG

        }
    }
}