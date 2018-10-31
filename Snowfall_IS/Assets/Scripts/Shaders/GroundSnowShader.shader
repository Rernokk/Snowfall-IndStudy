Shader "Custom/GroundSnowShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SnowTex("Snow Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Factor("Factor", Range(0, 1)) = 0.0
		_SnowAccum("Snow Accumulation", Range(0, 1)) = 0.0
		_SnowEdgeFactor("Snow Edge Factor", Range(0, .9)) = 0.5
	}

		SubShader
		{
			Cull Off
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
					sampler2D _SnowTex;
					float4 _MainTex_ST;
					float4 _SnowTex_ST;
					float4 _Color;
					float _Factor;
					float _SnowAccum;
					float _SnowEdgeFactor;

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
						o.diff = fixed3(0,0,0);
						o.ambient = ShadeSH9(half4(worldNormal, 1));
						TRANSFER_SHADOW(o)
						return o;
					}

					[maxvertexcount(3)]
					void geom(triangle v2g input[3], inout TriangleStream<g2f> TriStream) {
						g2f o;

						float4 worldNorm0 = mul(unity_ObjectToWorld, input[0].normal);
						float4 worldNorm1 = mul(unity_ObjectToWorld, input[1].normal);
						float4 worldNorm2 = mul(unity_ObjectToWorld, input[2].normal);
						float3 noise0 = float3(0, tex2Dlod(_SnowTex, fixed4(input[0].uv, 1, 1)).r, 0);
						float3 noise1 = float3(0, tex2Dlod(_SnowTex, fixed4(input[1].uv, 1, 1)).r, 0);
						float3 noise2 = float3(0, tex2Dlod(_SnowTex, fixed4(input[2].uv, 1, 1)).r, 0);

						float inf = saturate(dot(worldNorm0, fixed3(0, 1, 0)));
						inf = inf < _SnowEdgeFactor ? 0 : inf;
						float4 p0 = UnityObjectToClipPos(input[0].worldPos + (noise0 + input[0].normal) * _Factor * inf);

						inf = saturate(dot(worldNorm1, fixed3(0, 1, 0)));
						inf = inf < _SnowEdgeFactor ? 0 : inf;
						float4 p1 = UnityObjectToClipPos(input[1].worldPos + (noise1 + input[1].normal) * _Factor * inf);

						inf = saturate(dot(worldNorm2, fixed3(0, 1, 0)));
						inf = inf < _SnowEdgeFactor ? 0 : inf;
						float4 p2 = UnityObjectToClipPos(input[2].worldPos + (noise2 + input[2].normal) * _Factor * inf);

						float3 triNorm = normalize(cross(p1 - p0, p2 - p0));

						float normFac = 1;

						//o.vertex = mul(unity_ObjectToWorld, input[0].vertex - normalize((input[0].vertex - input[1].vertex) + (input[0].vertex - input[2].vertex)) * _Factor);
						//o.vertex = input[0].vertex;

						o.pos = p0;
						o.worldPos = input[0].worldPos;
						o.uv = input[0].uv;
						o.snowUV = input[0].snowUV;
						o.col = worldNorm0;
						o.normal = input[0].normal;
						o.diff = max(0, dot(worldNorm0, _WorldSpaceLightPos0.xyz)) * _LightColor0.rgb;
						o.ambient = ShadeSH9(half4(worldNorm0));
						TRANSFER_SHADOW(o);
						TriStream.Append(o);

						//o.vertex = mul(unity_ObjectToWorld, input[1].vertex - normalize((input[1].vertex - input[0].vertex) + (input[1].vertex - input[2].vertex)) * _Factor);
						//o.vertex = input[1].vertex;
						o.pos = p1;
						o.worldPos = input[1].worldPos;
						o.uv = input[1].uv;
						o.snowUV = input[1].snowUV;
						o.col = worldNorm1;
						o.normal = input[1].normal;
						o.diff = max(0, dot(worldNorm1, _WorldSpaceLightPos0.xyz)) * _LightColor0.rgb;
						o.ambient = ShadeSH9(half4(worldNorm1));
						TRANSFER_SHADOW(o);
						TriStream.Append(o);

						//o.vertex = mul(unity_ObjectToWorld, input[2].vertex - normalize((input[2].vertex - input[1].vertex)  + (input[2].vertex - input[0].vertex)) * _Factor);
						//o.vertex = input[2].vertex;
						o.pos = p2;
						o.worldPos = input[2].worldPos;
						o.uv = input[2].uv;
						o.snowUV = input[2].snowUV;
						o.col = worldNorm2;
						o.normal = input[2].normal;
						o.diff = max(0, dot(worldNorm2, _WorldSpaceLightPos0.xyz)) * _LightColor0.rgb;
						o.ambient = ShadeSH9(half4(worldNorm2));
						TRANSFER_SHADOW(o);
						TriStream.Append(o);

						TriStream.RestartStrip();
					}

					fixed4 frag(g2f i) : SV_Target
					{
						fixed4 col = (1 - _SnowAccum) * (tex2D(_MainTex, i.uv) * _Color) + saturate(dot(i.normal, fixed3(0,1,0))) * (_SnowAccum) * (tex2D(_SnowTex, i.snowUV));
						col = (1 - _SnowAccum) * (tex2D(_MainTex, i.uv) * _Color) + (_SnowAccum) * (tex2D(_SnowTex, i.snowUV) * saturate(tex2D(_SnowTex, i.snowUV + float2(.5,.5)).r + tex2D(_SnowTex, i.snowUV).r) * .5f);
						col = dot(i.normal, fixed3(0,1,0)) * col + (1 - dot(i.normal, fixed3(0,1,0))) * (tex2D(_MainTex, i.uv) * _Color);
						fixed shadow = SHADOW_ATTENUATION(i);
						fixed3 lighting = i.diff * shadow + i.ambient;
						col.rgb *= lighting;
						col.a = 1;
						return col;
					}
					ENDCG
				}

			Pass
			{
				Cull Off
				
				Tags {"LightMode" = "ShadowCaster"}
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_shadowcaster

				#include "UnityCG.cginc"

				struct appdata {
					float4 vertex : POSITION;
					float3 normal : NORMAL;
				};

				struct v2f {
					V2F_SHADOW_CASTER;
				};

				float _Factor;
				float _SnowEdgeFactor;
				sampler2D _SnowTex;

				v2f vert(appdata v) {
					v2f o;
					float4 worldVert = mul(unity_ObjectToWorld, v.vertex);
					float dist = worldVert.z - _WorldSpaceCameraPos.z;
					float inf = saturate(dot(v.normal, fixed3(0, 1, 0)));
					inf = inf < _SnowEdgeFactor ? 0 : 1;

					v.vertex += fixed4(fixed3(0,1,0) * _Factor, 0);
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