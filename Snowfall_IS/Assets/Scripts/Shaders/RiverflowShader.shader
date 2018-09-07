Shader "Custom/RiverflowShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_NormTex ("Normal", 2D) = "white" {}
		_FlowTex ("Flow Texture", 2D) = "black" {}

		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Rate ("Flow Rate", Range(0, 15)) = 0.0
		_Alpha ("River Transparency", Range(0, 1)) = 1.0
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
		sampler2D _NormTex;
		sampler2D _FlowTex;

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

		fixed2 ApplyFlowAlbedo(fixed2 flowSample, fixed2 textureLocation){
			fixed2 retVal = fixed2(0.,0.);
			
			//Lifespan
			const float half_period = .05;
			const float period = 2 * half_period;

			//Sample A
			float sampleA_Offset = fmod(_Time, period);
			float sampleA_Weight = sampleA_Offset / half_period;
			
			//Clamping to [0, 1]
			if (sampleA_Weight > 1.0) sampleA_Weight = 2.0 - sampleA_Weight;

			//Sample B
			float sampleB_Offset = fmod(_Time + half_period, period);
			float sampleB_Weight = 1.0 - sampleA_Weight;

			half2 sampA = tex2D(_FlowTex, textureLocation - (flowSample * sampleA_Offset));
			half2 sampB = tex2D(_FlowTex, textureLocation - (flowSample * sampleB_Offset));
			retVal += sampA * sampleA_Weight;
			retVal += sampB * sampleA_Weight;
			retVal = normalize(retVal);
			return retVal;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed2 flowmapSample = (2 * (tex2D(_FlowTex, IN.uv_MainTex).rg)) - 1;
<<<<<<< HEAD

			float timeSample = frac(_Time[1]);//frac(_Time[1]);
			timeSample = _Time[1];

			//fixed4 albedoSample = tex2D(_MainTex, IN.uv_MainTex + flowmapSample * _Rate * timeSample);
			fixed4 albedoSample = (tex2D(_FlowTex, IN.uv_MainTex) * .0f + 1.0f * tex2D(_MainTex, IN.uv_MainTex + flowmapSample * _Rate * timeSample));
			fixed4 normalSample = tex2D(_NormTex, IN.uv_MainTex + flowmapSample * _Rate * timeSample);
=======
			//smoothstep(flowmapSample, fixed2(0,0), .1);
			//float timeSample = frac(_Time[1]);//frac(_Time[1]);
			
			fixed2 sampleLocation = ApplyFlowAlbedo(flowmapSample, IN.uv_MainTex);

			//fixed4 albedoSample = tex2D(_MainTex, IN.uv_MainTex + flowmapSample * _Rate * timeSample);
			fixed4 albedoSample = tex2D(_MainTex, sampleLocation * .05f + IN.uv_MainTex);
>>>>>>> ec27d8324bf0e93858c5a512837575e128f853b1
			//fixed4 normalSample = tex2D(_NormTex, IN.uv_MainTex);

			//Display offset albedo
			o.Albedo = albedoSample.rgb;

			//Display Flow Map
			//o.Albedo = tex2D(_FlowTex, IN.uv_MainTex);
			//o.Alpha = 1;

			//Normal Map
			//o.Normal = normalSample.rgb;

			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = _Alpha;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
