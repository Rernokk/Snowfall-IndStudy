Shader "Custom/RiverflowShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormTex ("Normal", 2D) = "white" {}
		_FlowTex ("Flow Texture", 2D) = "black" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Rate("Flow Rate", Range(0, 20)) = 0.0
		_Debug("Debug Mode", Range(0, 1)) = 0
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
		fixed Per;
		fixed _Rate;
		fixed _Debug;

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
			if (_Debug > 0) {
				o.Albedo = (tex2D(_FlowTex, IN.uv_MainTex) + tex2D(_Overlay, IN.uv_MainTex)) * .5;
				o.Metallic = 0;
				o.Smoothness = 0;
				o.Alpha = 1;
				return;
			}
			//fixed4 samp = tex2D (_RateTex, fixed2(IN.uv_MainTex.x, 0));
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex + fixed2(0, _Time.y * samp.b)) * _Color;
			fixed2 flowmapSample = (2 * (tex2D(_FlowTex, IN.uv_MainTex).rg)) - 1;
			//fixed maskVal = tex2D(_MaskTex, IN.uv_MainTex).r;
			fixed maskVal = 1;
			//fixed2 pos = IN.uv_MainTex + ((tex2D(_FlowTex, IN.uv_MainTex).rg * 2) - fixed2(1, 1));
			fixed4 c = tex2D (_MainTex, flowmapSample.rg * _Time.x * maskVal * _Rate + IN.uv_MainTex) * _Color;
			o.Normal = tex2D (_NormTex, flowmapSample.rg * _Time.x * maskVal * _Rate + IN.uv_MainTex);
			o.Albedo = c.rgb;
			//o.Albedo = fixed3(IN.uv_MainTex.rg, 0);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = .5;
			//o.Alpha = 1;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
