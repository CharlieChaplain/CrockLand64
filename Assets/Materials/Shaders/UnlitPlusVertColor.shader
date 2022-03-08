
Shader "Custom/UnlitPlusVertColor" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Tint("Tint Color", Color) = (1,1,1,1)
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			Pass {
			Lighting On
				CGPROGRAM

					#pragma vertex vert
					#pragma fragment frag
					#include "UnityCG.cginc"

					struct v2f
					{
						fixed4 pos : SV_POSITION;
						half4 color : COLOR0; 
						half4 colorFog : COLOR1;
						float2 uv_MainTex : TEXCOORD0;
						half3 normal : TEXCOORD1;
					};

					float4 _MainTex_ST;
					fixed4 _Tint;
					uniform half4 unity_FogStart;
					uniform half4 unity_FogEnd;

					v2f vert(appdata_full v)
					{
						v2f o;

						o.pos = UnityObjectToClipPos(v.vertex);

						//Vertex lighting 
						o.color = v.color;

						float distance = length(mul(UNITY_MATRIX_MV,v.vertex));

						o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);

						//Fog
						float4 fogColor = unity_FogColor;

						float fogDensity = (unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart);
						o.normal.g = fogDensity;
						o.normal.b = 1;

						o.colorFog = fogColor;
						o.colorFog.a = clamp(fogDensity,0,1);

						return o;
					}

					sampler2D _MainTex;

					float4 frag(v2f IN) : COLOR
					{
						half4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
						half4 color = c * (IN.colorFog.a);
						color.rgb += IN.colorFog.rgb*(1 - IN.colorFog.a);
						color.rgb *= _Tint.rgb;
						return color;
					}
				ENDCG
			}
	}
}