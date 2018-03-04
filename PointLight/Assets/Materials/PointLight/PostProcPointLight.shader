Shader "PostEffect/PointLight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TexSize ("Resolution", vector) = (0, 0, 0, 0)
		
		_ShadowPX ("Shadow +X", 2D) = "white" {}
		_ShadowNX ("Shadow -X", 2D) = "white" {}
		_ShadowPY ("Shadow +Y", 2D) = "white" {}
		_ShadowNY ("Shadow -Y", 2D) = "white" {}
		_ShadowPZ ("Shadow +Z", 2D) = "white" {}
		_ShadowNZ ("Shadow -Z", 2D) = "white" {}
		
		_CameraField ("Camera Field of View", vector) = (0, 0, 0, 0)
		
		_Location ("Light Location", vector) = (0, 0, 0, 0)
		_Threshold ("Volumn Light Threshold", vector) = (0.5, 0.5, 0.5, 0.0)
		_Density ("Light Density", float) = 1.0
		_HighLightDensity ("High Light Density", float) = 0.5
		_Radius ("Light Radius", range(0, 50)) = 1.0
		
		_StepDist ("Distance per Step", range(0.3, 4)) = 0.3
		_MaxDist ("Max distance", range(1, 50)) = 50
		_Color ("Color", vector) = (1, 1, 1, 1)
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
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			
			float4 _TexSize;
			
			sampler2D _ShadowPX;
			sampler2D _ShadowNX;
			sampler2D _ShadowPY;
			sampler2D _ShadowNY;
			sampler2D _ShadowPZ;
			sampler2D _ShadowNZ;
			
			float4x4 _CameraTrans;
			float4 _CameraField;
			
			float3 _PX;
			float3 _PY;
			float3 _PZ;
			float3 _NX;
			float3 _NY;
			float3 _NZ;
			
			float4 _Location;
			float4 _Threshold;
			float _Density;
			float _HighLightDensity;
			float _Radius;
			
			float _StepDist;
			float _MaxDist;
			
			float4 _Color;
			
			float Pi = 3.141592653589793f;
			
			// In camera position.
			float4 sampleCube(float3 pt, float3 loc)
			{
				float3 drc = pt - loc; // To camera coordinate.
				float3 dir = normalize(drc);
				
				float3 p[] = {_PX, _PY, _PZ, _NX, _NY, _NZ};
				float3 ups[] = {_PY, _NZ, _PY, _PY, _PZ, _PY}; // The up direction of the texture.
				
				float maxDot = 0.0f;
				int maxI = -1;
				for(int i=0; i<6; i++)
				{
					if(dot(dir, p[i]) > maxDot)
					{
						maxDot = dot(dir, p[i]);
						maxI = i;
					}
				}
				
				// Normalize the projection of dir to ndir.
				float3 forward = p[maxI];
				float3 up = ups[maxI];
				float3 xdir = dir / maxDot;
				float3 tdir = xdir - forward;
				float2 uv = float2(dot(up, tdir), dot(cross(forward, up), tdir));
				// The magic number is 0.5 / sqrt(3).
				uv = 0.28867513459 * uv.yx + float2(0.5f, 0.5f);
				
				float color;
				if(maxI == 0) color = tex2D(_ShadowPX, uv);
				else if(maxI == 1) color = tex2D(_ShadowPY, uv);
				else if(maxI == 2) color = tex2D(_ShadowPZ, uv);
				else if(maxI == 3) color = tex2D(_ShadowNX, uv);
				else if(maxI == 4) color = tex2D(_ShadowNY, uv);
				else color = tex2D(_ShadowNZ, uv);
				
				return color;
			}
			
			fixed4 frag (v2f data) : SV_Target
			{
				fixed4 final = fixed4(0.0f, 0.0f, 0.0f, 0.0f);
				
				float2 screenCoord = 2.0f * (data.uv - float2(0.5f, 0.5f));
				float2 screenAngle = atan(screenCoord * tan(_CameraField.xy));
				float3 dir = float3(
					sin(screenAngle.x),
					sin(screenAngle.y),
					- cos(screenAngle.y) * cos(screenAngle.x));
				
				float maxLDist = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, data.uv)));
				
				for(float dist = _StepDist; dist <= _MaxDist; dist += _StepDist) if(dist < maxLDist * 100.0f)
				{
					float3 pt = dist * dir; // The point to calculate the light intensity.
					float lightDist = distance(pt, _Location);
					float depth = Linear01Depth(UNITY_SAMPLE_DEPTH(sampleCube(pt, _Location)));
					// Magic number... But it works!!!!
					if(lightDist < depth * 50.0f)
					{
						float sumDist = lightDist + dist;
						float rate = _Density / (sumDist * sumDist);
						// Take some high light...
						float exRate = _HighLightDensity / (lightDist * lightDist);
						final += _Color * (rate + exRate) * max(0.0f, (_Radius - lightDist) / _Radius);
					}	
				}
				
				// return final + tex2D(_MainTex, data.uv);
				return final;
			}
			ENDCG
		}
	}
}
