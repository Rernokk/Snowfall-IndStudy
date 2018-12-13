Shader "Custom/BushSnowShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormalTex("Normal Tex", 2D) = "white" {}
		_SnowTex ("Snow Texture", 2D) = "white" {}
		_AlphaCut("Alpha Cutoff", Range(0,1)) = 1.0
	}

	SubShader {
		
		//Cull Off
		ZTest Less

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormalTex;
		sampler2D _SnowTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _AlphaCut;
		half _BushSnowAmnt;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 s = tex2D (_SnowTex, IN.uv_MainTex);
			o.Albedo = saturate(_BushSnowAmnt - s.rgb * s.rgb) * s.rgb + saturate(1 - _BushSnowAmnt + s.rgb * s.rgb) * c.rgb;
			
			if (c.a > _AlphaCut){
				o.Alpha = 1;
			} else {
				o.Alpha = 0;
			}
			if (o.Alpha == 0) {
				discard;
			}
			//o.Normal = UnpackNormal(tex2D(_NormalTex, IN.uv_MainTex));

		}
		ENDCG
	}
	FallBack "Diffuse"
}
