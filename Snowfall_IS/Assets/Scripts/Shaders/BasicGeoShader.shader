// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BasicGeoShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SnowTex("Snow Texture", 2D) = "white" {}
		_Factor("Factor", Range(0, 0.05)) = 0.0
		_Color("Color", Color) = (1,1,1,1)
		_SnowAccum("Snow Accumulation", Range(0, 1)) = 0.0
	}
	
	SubShader
	{


		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma multi_compile_fwdbase

				#include "UnityCG.cginc"
				#include "AutoLight.cginc"
				#include "Lighting.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _SnowTex;
				float4 _SnowTex_ST;
				float _Factor;
				float _SnowAccum;
				float4 _Color;

				struct appdata {
					float4 pos : POSITION;
					float3 normal : NORMAL;
					float2 uv : TEXCOORD0;
					float4 worldPos : POSITION;
				};

				struct v2g {
					float4 pos : SV_POSITION;
					float4 worldPos : TEXCOORD1;
					float2 uv : TEXCOORD0;
					float2 snowUV: TEXCOORD2;
					float3 normal : NORMAL;
					fixed3 diff : COLOR0;
					fixed3 ambient : COLOR2;
					SHADOW_COORDS(3)
				};

				struct g2f {
					float4 pos : SV_POSITION;
					float4 worldPos : TEXCOORD1;
					float2 uv : TEXCOORD0;
					float2 snowUV : TEXCOORD2;
					fixed4 col : COLOR;
					fixed3 diff : COLOR1;
					fixed3 ambient : COLOR2;
					fixed3 normal : NORMAL;
					SHADOW_COORDS(3)
				};

				v2g vert(appdata v)
				{
					v2g o;
					o.pos = UnityObjectToClipPos(v.pos);
					o.worldPos = v.worldPos;
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					o.snowUV = TRANSFORM_TEX(v.uv, _SnowTex);
					o.normal = v.normal;
					fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
					half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
					o.diff = nl * _LightColor0.rgb;
					o.ambient = ShadeSH9(half4(worldNormal, 1));
					TRANSFER_SHADOW(o)
					return o;
				}

				[maxvertexcount(3)]
				void geom(triangle v2g input[3], inout TriangleStream<g2f> TriStream) {
					g2f o;

					float3 p0 = input[0].worldPos.xyz;
					float3 p1 = input[1].worldPos.xyz;
					float3 p2 = input[2].worldPos.xyz;
					float3 triNorm = normalize(cross(p1 - p0, p2 - p0)) * _Factor;

					float4 worldNorm0 = mul(unity_ObjectToWorld, input[0].normal);
					float4 worldNorm1 = mul(unity_ObjectToWorld, input[1].normal);
					float4 worldNorm2 = mul(unity_ObjectToWorld, input[2].normal);

					float normFac = 0;

					//o.vertex = mul(unity_ObjectToWorld, input[0].vertex - normalize((input[0].vertex - input[1].vertex) + (input[0].vertex - input[2].vertex)) * _Factor);
					//o.vertex = input[0].vertex;

					o.pos = UnityObjectToClipPos(input[0].worldPos + _Factor * normFac * fixed4(0, saturate(dot(worldNorm0.xyz, fixed3(0, 1, 0))), 0, 0));
					o.worldPos = input[0].worldPos + fixed4(0, 1, 0, 0);
					o.uv = input[0].uv;
					o.snowUV = input[0].snowUV;
					o.col = worldNorm0;
					o.normal = worldNorm0;
					o.diff = input[0].diff;
					o.ambient = input[0].ambient;
					TRANSFER_SHADOW(o);
					TriStream.Append(o);

					//o.vertex = mul(unity_ObjectToWorld, input[1].vertex - normalize((input[1].vertex - input[0].vertex) + (input[1].vertex - input[2].vertex)) * _Factor);
					//o.vertex = input[1].vertex;
					o.pos = UnityObjectToClipPos(input[1].worldPos + _Factor * normFac * fixed4(0, saturate(dot(worldNorm1.xyz, fixed3(0, 1, 0))), 0, 0));
					o.worldPos = input[1].worldPos + fixed4(0, 1, 0, 0);
					o.uv = input[1].uv;
					o.snowUV = input[1].snowUV;
					o.col = worldNorm1;
					o.normal = worldNorm1;
					o.diff = input[1].diff;
					o.ambient = input[1].ambient;
					TRANSFER_SHADOW(o);
					TriStream.Append(o);

					//o.vertex = mul(unity_ObjectToWorld, input[2].vertex - normalize((input[2].vertex - input[1].vertex)  + (input[2].vertex - input[0].vertex)) * _Factor);
					//o.vertex = input[2].vertex;
					o.pos = UnityObjectToClipPos(input[2].worldPos + _Factor * normFac * fixed4(0, saturate(dot(worldNorm2.xyz, fixed3(0, 1, 0))),0,0));
					o.worldPos = input[2].worldPos + fixed4(0, 1, 0, 0);
					o.uv = input[2].uv;
					o.snowUV = input[2].snowUV;
					o.col = worldNorm2;
					o.normal = worldNorm2;
					o.diff = input[2].diff;
					o.ambient = input[2].ambient;
					TRANSFER_SHADOW(o);
					TriStream.Append(o);

					TriStream.RestartStrip();
				}

				fixed4 frag(g2f i) : SV_Target
				{
					fixed4 col = (1 - _SnowAccum) * (tex2D(_MainTex, i.uv) * _Color) + saturate(dot(i.normal, fixed3(0,1,0))) * (_SnowAccum) * (tex2D(_SnowTex, i.snowUV));
					col = (1 - _SnowAccum) * (tex2D(_MainTex, i.uv) * _Color) + (_SnowAccum) * (tex2D(_SnowTex, i.snowUV) * saturate(tex2D(_SnowTex, i.snowUV + float2(.5,.5)).r + tex2D(_SnowTex, i.snowUV).r) * .5f);
					col = dot(i.normal, fixed3(0,1,0)) * col + (1 - dot(i.normal, fixed3(0,1,0))) * (tex2D(_MainTex, i.uv) * _Color);
					col.a = 1;
					fixed shadow = SHADOW_ATTENUATION(i);
					fixed3 lighting = i.diff * shadow + i.ambient;
					col.rgb *= lighting;
					return col;
				}
				ENDCG
			}

		Pass
		{
			Tags {"LightMode" = "ShadowCaster" }
							CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_shadowcaster
				#include "UnityCG.cginc"

				struct v2f {
					V2F_SHADOW_CASTER;
				};

			v2f vert(appdata_base v) {
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
	}
	Fallback "VertexLit"
}