Shader "Custom/TestGroundShader" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_SnowTex("Snow Texture", 2D) = "white" {}
		_SnowCover("Snow Cover", Range(0, 1)) = 0.0
		_OffsetAmnt ("Offset Amount", Range(0, 1)) = 0.3
	}
		SubShader{
			Tags { "RenderType" = "Opaque"  }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Lambert fullforwardshadows vertex:vert addshadow

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _SnowTex;

			struct Input {
				float2 uv_MainTex;
			};

			fixed4 _Color;
			fixed _SnowCover;
			fixed _OffsetAmnt;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void vert(inout appdata_full v) {
				float4 col = tex2Dlod(_SnowTex, v.texcoord * 10.f);
				v.vertex.xyz += v.normal * _SnowCover * _OffsetAmnt * col.r * col.r * (col.r >= _SnowCover ? 1 : 0);
			}

			void surf(Input IN, inout SurfaceOutput o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				fixed4 snowSamp = tex2D(_SnowTex, IN.uv_MainTex * 10.f);
				//o.Albedo = saturate(_SnowCover - c.rgb) + c.rgb * (1 - _SnowCover);
				o.Albedo = saturate(1 - _SnowCover) * (c.rgb) + saturate(_SnowCover) * snowSamp;

				// Metallic and smoothness come from slider variables
				//o.Metallic = _Metallic;
				//o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
