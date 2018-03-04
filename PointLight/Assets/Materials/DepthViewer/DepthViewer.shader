Shader "Hidden/DepthSampler"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex; // This should be a depth texture.
			sampler2D _CameraDepthTexture;
			
			
			float4 frag(v2f inp) : SV_Target
			{
				float sampleV = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, inp.uv.xy));
                float depth = Linear01Depth(sampleV); // This is the true depth.
				if(depth >= 0.5f) return float4(0, 0, 0, 0);
				return float4(depth, depth, depth, 1.0f);
			}
			ENDCG
		}
	}
}
