// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/BasicGeoShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SnowTex ("Snow Texture", 2D) = "white" {}
		_Factor ("Factor", Range(0, 3)) = 0.0
		_Color ("Color", Color) = (1,1,1,1)
		_SnowAccum ("Snow Accumulation", Range(0, 1)) = 0.0
	}
	SubShader
	{
		

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _SnowTex;
			float _Factor;
			float _SnowAccum;
			float4 _Color;
			
			struct appdata {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float4 worldPos : POSITION;
			};

			struct v2g {
				float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct g2f {
				float4 vertex : SV_POSITION;
				float4 worldPos : TEXCOORD1;
				float2 uv : TEXCOORD0;
				fixed4 col : COLOR;
				fixed3 normal : NORMAL;
			};

			v2g vert (appdata v)
			{
				v2g o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = v.worldPos;
				o.uv = v.uv;
				o.normal = v.normal;
				return o;
			}
			
			[maxvertexcount(3)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> TriStream) {
				g2f o;

				/*o.vertex = input[0].vertex + fixed4(input[0].normal * _Factor, 0);
				o.worldPos = input[0].worldPos;
				o.uv = input[0].uv;
				o.col = fixed4(input[0].normal, 1);
				TriStream.Append(o);
				
				o.vertex = input[1].vertex + fixed4(input[1].normal * _Factor, 0);
				o.worldPos = input[1].worldPos;
				o.uv = input[1].uv;
				o.col = fixed4(input[1].normal, 1);
				TriStream.Append(o);

				o.vertex = input[2].vertex + fixed4(input[2].normal * _Factor, 0);
				o.worldPos = input[2].worldPos;
				o.uv = input[2].uv;
				o.col = fixed4(input[2].normal, 1);
				TriStream.Append(o);*/

				float3 p0 = input[0].worldPos.xyz;
				float3 p1 = input[1].worldPos.xyz;
				float3 p2 = input[2].worldPos.xyz;
				float3 triNorm = normalize(cross(p1 - p0, p2 - p0)) * _Factor;

				float4 worldNorm0 = mul(unity_ObjectToWorld, input[0].normal);
				float4 worldNorm1 = mul(unity_ObjectToWorld, input[1].normal);
				float4 worldNorm2 = mul(unity_ObjectToWorld, input[2].normal);

				//o.vertex = mul(unity_ObjectToWorld, input[0].vertex - normalize((input[0].vertex - input[1].vertex) + (input[0].vertex - input[2].vertex)) * _Factor);
				//o.vertex = input[0].vertex;
				o.vertex = UnityObjectToClipPos(input[0].worldPos + _Factor * fixed4(0, saturate(dot(worldNorm0.xyz, fixed3(0, 1, 0))), 0, 0));
				o.worldPos = input[0].worldPos + fixed4(0, 1, 0, 0);
				o.uv = input[0].uv;
				o.col = worldNorm0;
				o.normal = worldNorm0;
				TriStream.Append(o);
				
				//o.vertex = mul(unity_ObjectToWorld, input[1].vertex - normalize((input[1].vertex - input[0].vertex) + (input[1].vertex - input[2].vertex)) * _Factor);
				//o.vertex = input[1].vertex;
				o.vertex = UnityObjectToClipPos(input[1].worldPos + _Factor * fixed4(0, saturate(dot(worldNorm1.xyz, fixed3(0, 1, 0))), 0, 0));
				o.worldPos = input[1].worldPos + fixed4(0, 1, 0, 0);
				o.uv = input[1].uv;
				o.col = worldNorm1;
				o.normal = worldNorm1;
				TriStream.Append(o);
				
				//o.vertex = mul(unity_ObjectToWorld, input[2].vertex - normalize((input[2].vertex - input[1].vertex)  + (input[2].vertex - input[0].vertex)) * _Factor);
				//o.vertex = input[2].vertex;
				o.vertex = UnityObjectToClipPos(input[2].worldPos + _Factor * fixed4(0, saturate(dot(worldNorm2.xyz, fixed3(0, 1, 0))),0,0));
				o.worldPos = input[2].worldPos + fixed4(0, 1, 0, 0);
				o.uv = input[2].uv;
				o.col = worldNorm2;
				o.normal = worldNorm2;
				TriStream.Append(o);

				TriStream.RestartStrip();
			}

			fixed4 frag (g2f i) : SV_Target
			{
				fixed4 col = (1 - _SnowAccum) * (tex2D(_MainTex, i.uv) * _Color) + saturate(dot(i.normal, fixed3(0,1,0))) * (_SnowAccum) * (tex2D(_SnowTex, i.uv));
				col = (1 - _SnowAccum) * (tex2D(_MainTex, i.uv) * _Color) + (_SnowAccum) * (tex2D(_SnowTex, i.uv));
				col = dot(i.normal, fixed3(0,1,0)) * col + (1 - dot(i.normal, fixed3(0,1,0))) * (tex2D(_MainTex, i.uv) * _Color);
				col.a = 1;
				return col;
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
