﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/DepthTestShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{

		Tags {
			"RenderType" = "Opaque"
		}

		ZWrite On
	
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float depth : DEPTH;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z *_ProjectionParams.w;
				return o;
			}
			
			half4 _Color;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed col = (1 - i.depth);
				return fixed4(col,col,col, 1);
			}
			ENDCG
		}
	}
}
