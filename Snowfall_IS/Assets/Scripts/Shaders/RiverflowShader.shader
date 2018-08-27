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
			fixed offSet = 0.015;
			fixed2 sample0 = (2 * (tex2D(_FlowTex, IN.uv_MainTex + fixed2(0, offSet)).rg)) - 1;
			fixed2 sample1 = (2 * (tex2D(_FlowTex, IN.uv_MainTex + fixed2(0, -offSet)).rg)) - 1;
			fixed2 sample2 = (2 * (tex2D(_FlowTex, IN.uv_MainTex + fixed2(offSet, 0)).rg)) - 1;
			fixed2 sample3 = (2 * (tex2D(_FlowTex, IN.uv_MainTex + fixed2(-offSet, 0)).rg)) - 1;

			fixed2 sample4 = (2 * (tex2D(_FlowTex, IN.uv_MainTex + fixed2(offSet, offSet)).rg)) - 1;
			fixed2 sample5 = (2 * (tex2D(_FlowTex, IN.uv_MainTex + fixed2(-offSet, offSet)).rg)) - 1;
			fixed2 sample6 = (2 * (tex2D(_FlowTex, IN.uv_MainTex + fixed2(offSet, -offSet)).rg)) - 1;
			fixed2 sample7 = (2 * (tex2D(_FlowTex, IN.uv_MainTex + fixed2(-offSet, -offSet)).rg)) - 1;

			flowmapSample = (sample0 + sample1 + sample2 + sample3 + sample4 + sample5 + sample6 + sample7) * .125;

			fixed maskVal = tex2D(_MaskTex, IN.uv_MainTex).r;

			_Per0 = (_Time.x);
			_Per1 = smoothstep(_Time.x, (_Time.x + .37272), unity_DeltaTime.x);

			//_Per = 1;
			fixed4 c = (tex2D(_MainTex, flowmapSample.rg * _Per0 * maskVal * _Rate + IN.uv_MainTex) + tex2D(_MainTex, flowmapSample.rg * _Per1 * maskVal * _Rate + fixed2(.1235123, .83733) + IN.uv_MainTex)) * _Color;
			//o.Normal = tex2D (_NormTex, flowmapSample.rg * _Per0 * maskVal * _Rate + IN.uv_MainTex) + tex2D(_NormTex, flowmapSample.rg * _Per1 *.5 * maskVal * _Rate + float2(.5, .5) + IN.uv_MainTex);
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
