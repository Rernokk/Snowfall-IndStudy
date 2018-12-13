Shader "Hidden/DistortionShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_DistortionRate("Distortion Rate", Range(0,1)) = 1.0
		_OffsetTex ("Offset Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		GrabPass{
			"_GrabTex"
		}

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
				float4 screenUV : TEXCOORD1;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenUV.xy = (float2(o.vertex.x, -o.vertex.y) + o.vertex.w) * .5f;
				o.screenUV.zw = o.vertex.w;
				//o.screenUV.z /= distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _OffsetTex;
			sampler2D _GrabTex;
			half _DistortionRate;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 offset = (UnpackNormal(tex2D(_OffsetTex, i.uv + _Time.x * _DistortionRate)) + UnpackNormal(tex2D(_OffsetTex, i.uv + fixed2(_Time.x, -_Time.x) * _DistortionRate))) * .5f;
				fixed4 col = tex2Dproj(_GrabTex, UNITY_PROJ_COORD(i.screenUV + fixed4(offset.rg,0,0) * .75f * (.00015f * length(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, i.vertex)))));
				col.a = 1;
				return col;
			}
			ENDCG
		}
	}
}
