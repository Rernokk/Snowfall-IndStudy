Shader "Custom/GroundSnowShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SnowTex("Snow Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Factor("Factor", Range(0, 10)) = 0.0
		_SnowAccum("Snow Accumulation", Range(0, 1)) = 0.0
		_SnowEdgeFactor("Snow Edge Factor", Range(0, .9)) = 0.5
	}

		SubShader
		{
			Pass
			{
				Tags{ "LightMode" = "ForwardBase" }
				CGPROGRAM
					#pragma vertex vert
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

					struct v2f {
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

					v2f vert(appdata_base v)
					{
						v2f o;
						o.uv = v.texcoord;
						o.snowUV = v.texcoord;
						o.normal = v.normal;
						o.pos = UnityObjectToClipPos(v.vertex + v.normal * _Factor);
						fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
						half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
						o.diff = nl * _LightColor0.rgb;
						o.ambient = ShadeSH9(half4(worldNormal, 1));
						TRANSFER_SHADOW(o)
						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						fixed4 col = (1 - _SnowAccum) * (tex2D(_MainTex, i.uv) * _Color) + saturate(dot(i.normal, fixed3(0,1,0))) * (_SnowAccum) * (tex2D(_SnowTex, i.snowUV));
						col = (1 - _SnowAccum) * (tex2D(_MainTex, i.uv) * _Color) + (_SnowAccum) * (tex2D(_SnowTex, i.snowUV) * saturate(tex2D(_SnowTex, i.snowUV + float2(.5,.5)).r + tex2D(_SnowTex, i.snowUV).r) * .5f);
						col = dot(i.normal, fixed3(0,1,0)) * col + (1 - dot(i.normal, fixed3(0,1,0))) * (tex2D(_MainTex, i.uv) * _Color);
						fixed shadow = SHADOW_ATTENUATION(i);
						fixed3 lighting = (i.diff + i.ambient) * shadow;
						//fixed3 lighting = i.diff * shadow + i.ambient;
						col.rgb *= lighting;
						col.a = 1;
						return col;
					}
					ENDCG
				}

			Pass
			{
				//Cull Off

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
					/*float4 worldVert = mul(unity_ObjectToWorld, v.vertex);
					float dist = worldVert.z - _WorldSpaceCameraPos.z;
					float inf = saturate(dot(v.normal, fixed3(0, 1, 0)));
					inf = inf < _SnowEdgeFactor ? 0 : 1;
					v.vertex += fixed4(UnityObjectToWorldNormal(v.normal) * _Factor * inf, 0);*/
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