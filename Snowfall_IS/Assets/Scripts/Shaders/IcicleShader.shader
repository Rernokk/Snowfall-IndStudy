// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/IcicleShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_NoiseFactor("Noise Factor", Range(0,1)) = 0.0
		_NoiseMax("Noise Max", float) = 0.0
		_NoiseFalloff("Noise Falloff", float) = 1.0
		_Lowest("Lowest Noise Value", float) = 0.0
		_Highest("Highest Noise Value", float) = 1.0
	}
		SubShader
		{
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha

			Pass
			{
				//Tags{ "LightMode" = "ForwardBase" }
				Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
				CGPROGRAM
				#pragma vertex vert
				#pragma geometry geom
				#pragma fragment frag
				#pragma multi_compile_fwdbase

				#include "UnityCG.cginc"
				#include "AutoLight.cginc"
				#include "Lighting.cginc"

				struct appdata
				{
					float4 pos : POSITION;
					float2 uv : TEXCOORD0;
					float4 normal : NORMAL;
					float3 viewDir : TEXCOORD1;
				};

				struct v2g
				{
					float2 uv : TEXCOORD0;
					float4 pos : SV_POSITION;
					float4 normal : NORMAL;
					float3 viewDir : TEXCOORD1;
					SHADOW_COORDS(2)
				};

				struct g2f {
					float2 uv : TEXCOORD0;
					float4 pos : POSITION;
					float3 color : COLOR;
					SHADOW_COORDS(2)
				};

				v2g vert(appdata v)
				{
					v2g o;
					o.pos = v.pos;
					o.normal = v.normal;
					o.uv = v.uv;
					o.viewDir = mul(unity_ObjectToWorld, v.pos).xyz - _WorldSpaceCameraPos.xyz;
					//TRANSFER_SHADOW(o);
					return o;
				}


				sampler2D _MainTex;
				sampler2D _NoiseTex;
				float _NoiseFactor;
				float _NoiseMax;
				float _NoiseFalloff;
				float _Highest;
				float _Lowest;

				[maxvertexcount(12)]
				void geom(triangle v2g input[3], inout TriangleStream<g2f> TriStream) {
					g2f o;

					float noiseSamp;
					float4 ptFin = fixed4(0, -1, 0, 0) * _NoiseFactor;
					o.color = fixed4(1, 1, 1, 1);
					float2 avgUV = input[0].uv + input[1].uv + input[2].uv;
					avgUV *= .33333;

					/*if (avgUV.x <= 0.001 || avgUV.x >= .999) {
						ptFin = float4(0, 0, 0, 0);
					}*/

					noiseSamp = saturate((tex2Dlod(_NoiseTex, fixed4(input[0].uv, 0, 0)).r - _Lowest) * (1 / (_Highest - _Lowest)));
					o.uv = input[0].uv;
					//o.pos = UnityObjectToClipPos(input[0].pos + ptFin * noiseSamp * _NoiseMax);
					o.pos = UnityObjectToClipPos(mul(unity_WorldToObject, mul(unity_ObjectToWorld, input[0].pos) + ptFin * noiseSamp * _NoiseMax));
					TRANSFER_SHADOW(o);
					TriStream.Append(o);

					noiseSamp = saturate((tex2Dlod(_NoiseTex, fixed4(input[1].uv, 0, 0)).r - _Lowest) * (1 / (_Highest - _Lowest)));
					o.uv = input[1].uv;
					//o.pos = UnityObjectToClipPos(input[1].pos + ptFin * noiseSamp * _NoiseMax);
					o.pos = UnityObjectToClipPos(mul(unity_WorldToObject, mul(unity_ObjectToWorld, input[1].pos) + ptFin * noiseSamp * _NoiseMax));
					TRANSFER_SHADOW(o);
					TriStream.Append(o);

					noiseSamp = saturate((tex2Dlod(_NoiseTex, fixed4(input[2].uv, 0, 0)).r - _Lowest) * (1 / (_Highest - _Lowest)));
					o.uv = input[2].uv;
					//o.pos = UnityObjectToClipPos(input[2].pos + ptFin * noiseSamp * _NoiseMax);
					o.pos = UnityObjectToClipPos(mul(unity_WorldToObject, mul(unity_ObjectToWorld, input[2].pos) + ptFin * noiseSamp * _NoiseMax));
					TRANSFER_SHADOW(o);
					TriStream.Append(o);

					TriStream.RestartStrip();
				}



				fixed4 frag(g2f i) : SV_Target
				{
					fixed4 col = tex2D(_NoiseTex, i.uv);
					if (col.r > 0.03) {
						//col.a = 1 - col.r * col.r;
						col.a = 1;
					}
					else {
						col.a = 0;
					}
					return col;
				}
				ENDCG
			}
		}
}
