Shader "Custom/VertexLitBlendTransparent" {
	Properties{
		_MainTex1("Base (RGB)", 2D) = "white" {}
		_MainTex2("Base (RGB)", 2D) = "white" {}
		_Tint("Tint Color", Color) = (1,1,1,1)
	}
		SubShader{
			Tags { "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }
			LOD 200

			Pass {
			Lighting On

			Tags{ "LIGHTMODE" = "Vertex" "QUEUE" = "Transparent" "IGNOREPROJECTOR" = "true" "RenderType" = "Transparent" }
			ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha
				ColorMask RGB
				CGPROGRAM

					#pragma vertex vert
					#pragma fragment frag
					#include "UnityCG.cginc"

					struct v2f
					{
						fixed4 pos : SV_POSITION;
						half4 color : COLOR0;
						half4 colorFog : COLOR1;
						half4 lerpValue : COLOR2; // uses vertex color (grayscale) to lerp between texture
						float2 uv_MainTex : TEXCOORD0;
						half3 normal : TEXCOORD1;
					};

					float4 _MainTex1_ST;
					float4 _MainTex2_ST;
					fixed4 _Tint;
					uniform half4 unity_FogStart;
					uniform half4 unity_FogEnd;

					v2f vert(appdata_full v)
					{
						v2f o;

						o.pos = UnityObjectToClipPos(v.vertex);

						//Vertex lighting 
						o.color = float4(ShadeVertexLightsFull(v.vertex, v.normal, 4, true), v.color.a);

						//transfers lerpvalue via vert color
						o.lerpValue = v.color;

						float distance = length(mul(UNITY_MATRIX_MV,v.vertex));

						o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex1);

						//Fog
						float4 fogColor = unity_FogColor;

						float fogDensity = (unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart);
						o.normal.g = fogDensity;
						o.normal.b = 1;

						o.colorFog = fogColor;
						o.colorFog.a = clamp(fogDensity,0,1);

						return o;
					}

					sampler2D _MainTex1;
					sampler2D _MainTex2;

					float4 frag(v2f IN) : COLOR
					{
						half4 c = lerp(tex2D(_MainTex1, IN.uv_MainTex), tex2D(_MainTex2, IN.uv_MainTex), IN.lerpValue) * IN.color;
						half4 color = c * (IN.colorFog.a);
						color.rgb += IN.colorFog.rgb * (1 - IN.colorFog.a);
						color.rgb *= _Tint.rgb;
						color.a = IN.color.a;
						return color;
					}
				ENDCG
			}
		}
}