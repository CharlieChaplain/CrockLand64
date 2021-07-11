Shader "Custom/sphereMapFog"
{
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Environment Texture", 2D) = "white" {}
	}
		SubShader{
			Tags {
				"Queue" = "Geometry"
				"RenderType" = "Opaque"
			}
			LOD 200
		Pass{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _Color;
			uniform half4 unity_FogStart;
			uniform half4 unity_FogEnd;

			struct vertInput {
				float4 pos: POSITION;
				float3 normal: NORMAL;
			};

			struct vertOutput {
				float4 pos: SV_POSITION;
				float2 uv: TEXCOORD0;
				half4 colorFog : COLOR1;
				half3 normal : TEXCOORD1;
			};

			vertOutput vert(vertInput input) {
				vertOutput output;

				output.pos = UnityObjectToClipPos(input.pos);

				//calculating uv
				float4x4 modelMatrix = unity_ObjectToWorld;
				float4x4 modelMatrixInverse = unity_WorldToObject;

				float3 viewDir = normalize(_WorldSpaceCameraPos - mul(modelMatrix, input.pos).xyz);
				float3 normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				float3 r = normalize(reflect(viewDir, normalDir));

				float M = 2 * sqrt(pow(r.x, 2.0) + pow(r.y, 2.0) + pow(r.z + 1.0, 2.0));

				float u = (r.x / M) + 0.5;
				float v = (r.y / M) + 0.5;

				output.normal = input.normal;

				//Fog
				//float distance = length(mul(UNITY_MATRIX_MV, input.pos));
				float distance = length(UnityObjectToViewPos(input.pos));

				float4 fogColor = unity_FogColor;

				float fogDensity = (unity_FogEnd - distance) / (unity_FogEnd - unity_FogStart);
				output.normal.g = fogDensity;
				output.normal.b = 1;

				output.colorFog = fogColor;
				output.colorFog.a = clamp(fogDensity, 0, 1);

				output.uv = float2(u, v);
				return output;
			}

			float4 frag(vertOutput input) : COLOR
			{
				half4 c = tex2D(_MainTex, input.uv);
				half4 color = c * (input.colorFog.a);
				color.rgb += input.colorFog.rgb*(1 - input.colorFog.a);
				return color;

				//return tex2D(_MainTex, input.uv);
			}


			ENDCG
		} }
		FallBack "Diffuse"
}