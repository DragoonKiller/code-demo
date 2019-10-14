Shader "Hidden/Flow-4"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _Scale ("Scale", vector) = (1, 1, 0, 0)
        _Freq ("Freq", float) = 1
        _Bias ("Bias", float) = 1
        _Speed ("Speed", vector) = (1, 1, 0, 0)
        _SpeedMult ("Speed Mult", float) = 1.5
        _TransparentMult ("Transparent Mult", float) = 0.8
        _IlluminationMult ("Illumination Mult", float) = 1
        _ExposureMult ("Exposure Mult", float) = 1
        _BiasMult ("Bias Mult", float) = 1
        _Repeat ("Repeat", int) = 1
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

            // L 后缀: 物体局部坐标系 (_) (默认)
            // W 后缀: 世界坐标系 (M)
            // V 后缀: 相机坐标系 (MV)
            // N 后缀: 裁剪视窗坐标系 (MVP)
            // F 后缀: 屏幕像素坐标系
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
                float4 color : COLOR0;
            };

            sampler2D _MainTex;
            float2 _Scale;
            float _Freq;
            float _Bias;
            float4 _Speed; // x: 图像移动速度. y: 折叠移动速度.
            float _SpeedMult;
            float _TransparentMult;
            float _IlluminationMult;
            float _ExposureMult;
            float _BiasMult;
            int _Repeat;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            float4 frag (v2f i) : SV_Target
            {
                float4 color = float4(0, 0, 0, 0);
                float sum = 0;
                for(int t = 1; t <= _Repeat; t++)
                {
                    float2 st = xmap(i.uv, 0, 1, -1, 1) / _Scale;
                    st.y += _Speed.y * _Time.y * pow(_SpeedMult, t);
                    float2 uv = xmap(st, -1, 1, 0, 1);
                    uv.y += _Speed.x * _Time.y * pow(2, t);
                    float noise = perlinNoise(float3(st, _Speed.z * _Time.y), _Freq);
                    float bias = _Bias * pow(_BiasMult, t);
                    float4 cColor = tex2D(_MainTex, float2(uv.x + xmap(noise, 0, 1, -bias, bias), uv.y)) * pow(_TransparentMult, t);
                    color += float4(_ExposureMult * cColor.rgb * pow(_IlluminationMult, t), cColor.a);
                    sum += pow(_TransparentMult, t);
                }
                
                color /= sum;                
                return color * i.color;
            }
            ENDCG
        }
    }
}
