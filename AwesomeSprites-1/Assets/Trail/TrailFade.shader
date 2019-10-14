Shader "Hidden/trailfade-1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _Aspect ("Aspect", float) = 1
        _Rate ("Rate", float) = 0
        _Spread ("Spread", float) = 1
        _Bias ("Bias", vector) = (1, 1, 0, 0)
        _BiasCount ("Bias Count", int) = 3
        _BiasMult ("Bias Mult", float) = 1.0
        _BiasFreq ("Bias Frequency", float) = 20
    }
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Tags { "RenderQueue" = "Transparent" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Assets/Utils/Utils.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Aspect;
            float _Rate;
            float _Spread;
            float4 _Bias;
            float _BiasFreq;
            float _BiasMult;
            int _BiasCount;
            
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = float2(xmap(xmap(v.uv.x, 0, 1, -1, 1) * _Aspect, -1, 1, 0, 1), v.uv.y);
                o.color = v.color;
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float biasOffsets[] = {116.42, 112.35, 337.98, 43.12, 55.69, 77.74, 35.976, 63.428};
                
                float rate = _Rate + 1 - i.uv.y;
                
                float2 uv = i.uv;
                uv.x = xmap(uv.x, 0, 1, -1, 1);
                uv.x = uv.x * exp(-rate * _Spread);
                uv.x = xmap(uv.x, -1, 1, 0, 1);
                
                float4 color = float4(0, 0, 0, 0);
                float sum = 0;
                for(int t = 0; t < _BiasCount; t++)
                {
                    float2 bias = float2(
                        perlinNoise(float3(uv + float2(biasOffsets[t], biasOffsets[t + 1]), _Time.y * _Bias.z), _BiasFreq),
                        perlinNoise(float3(uv + float2(biasOffsets[t + 1], biasOffsets[t + 2]), _Time.y * _Bias.z), _BiasFreq)
                    );
                    bias.x = xmap(bias.x, 0, 1, -_Bias.x * rate, _Bias.x * rate);
                    bias.y = xmap(bias.y, 0, 1, -_Bias.y * rate, _Bias.y * rate);
                    color += i.color * tex2D(_MainTex, uv + bias) * pow(_BiasMult, t);
                    sum += pow(_BiasMult, t);
                }
                color /= sum;
                
                color.a *= 1 - rate;
                return color;
            }
            ENDCG
        }
    }
}
