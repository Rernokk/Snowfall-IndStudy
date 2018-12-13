Shader "Hidden/BushVFShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_SnowTex ("Snow Texture", 2D) = "white" {}
		//_BushSnowAmnt ("Snow Amount", Range(0,1)) = 0.0
		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.0
	}
		SubShader
		{
			Cull Off

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				sampler2D _MainTex;
				sampler2D _SnowTex;
				half _Cutoff;
				half _BushSnowAmnt;

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 c = tex2D(_MainTex, i.uv);
					c.rgb /= 5.f * (_BushSnowAmnt + .1f);
					fixed4 s = tex2D(_SnowTex, i.uv);
					fixed4 col = fixed4(saturate(_BushSnowAmnt - s.rgb * s.rgb) * s.rgb + saturate(1 - _BushSnowAmnt + s.rgb * s.rgb) * c.rgb, c.a);
					if (c.a < _Cutoff) {
						discard;
					}
					return col;
				}
				ENDCG
			}
			UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
		}
}
