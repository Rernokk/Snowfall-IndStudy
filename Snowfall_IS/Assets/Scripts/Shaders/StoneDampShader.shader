Shader "Custom/StoneDampShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_SpecCol ("Specular Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormTex ("Normal Texture", 2D) = "white" {}
		_SnowTex ("Snow Texture", 2D) = "white" {}
		_Occlusion("Occlusion", float) = 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormTex;
		sampler2D _SnowTex;

		struct Input {
			float2 uv_MainTex;
		};

		uniform half _StoneGloss;
		half _Occlusion;
		fixed4 _Color;
		fixed3 _SpecCol;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandardSpecular o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 s = tex2D (_SnowTex, IN.uv_MainTex);
			o.Albedo = _StoneGloss * .5f * s.rgb + (1 - _StoneGloss * .5f) * c.rgb;
			o.Normal = UnpackNormal(tex2D(_NormTex, IN.uv_MainTex));
			o.Specular = _SpecCol * .5f * o.Albedo;
			o.Smoothness = _StoneGloss;
			o.Occlusion = _Occlusion;
			o.Alpha = 1.0f;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
