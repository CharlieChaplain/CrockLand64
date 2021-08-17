
Shader "Custom/GemShader/normal" {
	Properties{
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
						half3 normal : TEXCOORD1;
					};

					fixed4 _Tint;
					uniform half4 unity_FogStart;
					uniform half4 unity_FogEnd;

					v2f vert(appdata_full v)
					{
						v2f o;

						o.pos = UnityObjectToClipPos(v.vertex);

						o.color = v.color;

						float distance = length(mul(UNITY_MATRIX_MV,v.vertex));

						//Fog
						float4 fogColor = unity_FogColor;

						float fogDensity = (unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart);
						//o.normal.g = fogDensity;
						//o.normal.b = 1;

						o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);

						o.colorFog = fogColor;
						o.colorFog.a = clamp(fogDensity,0,1);

						return o;
					}

					sampler2D _MainTex;

					float4 frag(v2f IN) : COLOR
					{
						float3 camViewDir = mul((float3x3)unity_CameraToWorld, float3(0,0,1));
						//float3 lightDir = _WorldSpaceLightPos0.xyz;

						//ensmallens the radius of the shine
						float3 normalFixed = -IN.normal.xyz - (camViewDir * 0.3);
						//float3 normalFixed = -IN.normal.xyz;
						float lightDot = clamp(dot(normalFixed, camViewDir), 0, 1);

						half4 c = IN.color;
						//a lighter version of the vertex color
						//half4 cLight = c + half4(.9, .9, .9, 1);
						half4 cLight = half4(2, 2, 2, 1);

						half4 color = lerp(c, cLight, lightDot);

						color *= (IN.colorFog.a);
						color.rgb += IN.colorFog.rgb*(1 - IN.colorFog.a);
						color.rgb *= _Tint.rgb;

						//float num = lightDot;
						//color = half4(num, 0, num, 1);

						//half3 num3 = IN.normal;
						//color = half4(num3, 1);

						return color;
					}
				ENDCG
			}
	}
}