Shader "Custom/TerrainPlanarMapping" {
	Properties {
	// Control Texture ("Splat Map")
	[HideInInspector] _Control("Control (RGBA)", 2D) = "red" {}

	// Terrain textures - each weighted according to the corresponding colour
	// channel in the control texture
	[HideInInspector] _Splat3("Layer 3 (A)", 2D) = "white" {}
	[HideInInspector] _Splat2("Layer 2 (B)", 2D) = "white" {}
	[HideInInspector] _Splat1("Layer 1 (G)", 2D) = "white" {}
	[HideInInspector] _Splat0("Layer 0 (R)", 2D) = "white" {}

	// Used in fallback on old cards & also for distant base map
	[HideInInspector] _MainTex("BaseMap (RGB)", 2D) = "white" {}
	[HideInInspector] _Color("Main Color", Color) = (1,1,1,1)
	
		_SnowTex ("Snow Texture", 2D) = "grey" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0


		uniform sampler2D _Control;
		uniform sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
		uniform fixed4 _Color;
		uniform sampler2D _Ramp;

		sampler2D _MainTex;
		sampler2D _SnowTex;

		struct Input {
			float2 uv_Control : TEXCOORD0;
			float2 uv_Splat0 : TEXCOORD1;
			float2 uv_Splat1 : TEXCOORD2;
			float2 uv_Splat2 : TEXCOORD3;
			float2 uv_Splat3 : TEXCOORD4;
		};
		half _Glossiness;
		half _Metallic;
		fixed _Norm;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_base v) {
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_Control);
			fixed4 snowSamp = tex2D(_SnowTex, IN.uv_Control);
			o.Albedo = snowSamp.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
