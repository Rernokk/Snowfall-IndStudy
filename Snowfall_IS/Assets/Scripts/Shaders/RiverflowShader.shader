Shader "Custom/RiverflowShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormTex ("Normal", 2D) = "white" {}
		_FlowTex ("Flow Texture", 2D) = "black" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Rate("Flow Rate", Range(0, 20)) = 0.0
		_Alpha ("Alpha", Range(0, 1)) = 0.5
		_Debug("Debug Mode", Range(0, 1)) = 0
		//_Per("Test Period", Range(0, 1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _RateTex;
		sampler2D _NormTex;
		sampler2D _FlowTex;
		sampler2D _ObsTex;
		sampler2D _MaskTex;
		sampler2D _Overlay;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed _Per0;
		fixed _Per1;
		fixed _Rate;
		fixed _Debug;
		fixed _Alpha;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_tan data) {
			fixed4 dat = data.vertex ;
			data.vertex = dat;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {

			fixed2 flowmapSample = (2 * (tex2D(_FlowTex, IN.uv_MainTex).rg)) - 1;
			//flowmapSample += fixed2(0, 1);
			fixed maskVal = tex2D(_MaskTex, IN.uv_MainTex).r;

			_Per0 = _Time.x % 1;

			fixed4 c = (tex2D(_MainTex, flowmapSample.rg * _Per0 * maskVal * _Rate + IN.uv_MainTex)) * _Color;// + tex2D(_MainTex, flowmapSample.rg * _Per1 * maskVal * _Rate + fixed2(.1235123, .83733) + IN.uv_MainTex)) * _Color;
			o.Normal = (tex2D(_NormTex, flowmapSample.rg * _Per0 * maskVal * _Rate + IN.uv_MainTex));
			o.Albedo = c.rgb;
			//o.Albedo = tex2D(_FlowTex, IN.uv_MainTex);

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = _Alpha;
			//o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
