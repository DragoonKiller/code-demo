Shader "Hidden/GaussianBlurHorz"
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
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
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

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float2 _ScreenSize;
			
			fixed4 frag (v2f inp) : SV_Target
			{
				float2 coord = inp.uv * _ScreenSize; // on-screen coordinates.
				
				int n = 15;
				float f[] = {725, 625, 510, 400, 302, 245, 187, 142, 101, 77, 54, 65, 31, 18, 6};
				float sum = 0;
				for(int i=0; i<n; i++) sum += f[i];
				sum = sum * 2 - f[0];
				
				float4 final = float4(0, 0, 0, 1);
				for(int i = -n + 1; i<n; i++)
				{
					int g = f[abs(i)];
					final += tex2D(_MainTex, float2(coord.x, coord.y + i) / _ScreenSize) * g / sum;
				}
				return float4(final.xyz, 1.0f);
			}
			ENDCG
		}
	}
}
