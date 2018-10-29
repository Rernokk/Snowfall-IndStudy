Shader "Custom/RiverflowShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormTex ("Normal", 2D) = "white" {}
		_FlowTex ("Flow Texture", 2D) = "black" {}
		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_GlossMap("Gloss Map", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Rate ("Flow Rate", Range(0, 3)) = 0.0
		_Alpha ("River Transparency", Range(0, 1)) = 1.0
	}

	SubShader {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		LOD 200


		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NormTex;
		sampler2D _FlowTex;
		sampler2D _NoiseTex;
		sampler2D _GlossMap;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		fixed _Rate;
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
			data.vertex = UnityPixelSnap(data.vertex);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed2 flowmapSample = (2 * (tex2D(_FlowTex, IN.uv_MainTex).rg)) - 1;
			fixed noiseSample = tex2D(_NoiseTex, IN.uv_MainTex).r;

			float timeSample = frac(_Time[1] * _Rate);
			float timeSampleTwo = frac(_Time[1] * _Rate + .5);

			fixed4 albedoSample = (tex2D(_MainTex, IN.uv_MainTex + flowmapSample * timeSample));
			fixed4 albedoSampleTwo = (tex2D(_MainTex, IN.uv_MainTex + flowmapSample * timeSampleTwo));
			fixed4 normalSampleOne = tex2D(_NormTex, IN.uv_MainTex + flowmapSample * _Rate * timeSample);
			fixed4 normalSampleTwo = tex2D(_NormTex, IN.uv_MainTex + flowmapSample * _Rate * timeSampleTwo);

			//Display offset albedo
			o.Albedo = lerp(albedoSample.rgb, albedoSampleTwo.rgb, 2 * abs(timeSample - .5f)) * _Color;


			//Normal Map
			o.Normal = lerp(normalSampleOne.rgb, normalSampleTwo.rgb,2 * abs(timeSample - .5f));

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			
			float glossSampleOne = tex2D(_GlossMap, IN.uv_MainTex + flowmapSample * timeSample).r;
			float glossSampleTwo = tex2D(_GlossMap, IN.uv_MainTex + flowmapSample * timeSampleTwo).r;
			o.Smoothness = _Glossiness * lerp(glossSampleOne, glossSampleTwo, 2 * abs(timeSample-.5f));
			o.Alpha = _Alpha;


			//Debug
			/*o.Albedo = tex2D(_FlowTex, IN.uv_MainTex);
			o.Alpha = 1;
			o.Smoothness = 0;
			o.Metallic = 0;*/
			
		}
		ENDCG
	}
	FallBack "Diffuse"
}

//Reference Materials
/*

https://mtnphil.wordpress.com/2012/08/25/water-flow-shader/
http://twvideo01.ubm-us.net/o1/vault/gdc2012/slides/Missing%20Presentations/Added%20March%2026/Keith_Guerrette_VisualArts_TheTricksUp.pdf
https://developer.valvesoftware.com/wiki/Water_(shader)#Flowing_water

*/