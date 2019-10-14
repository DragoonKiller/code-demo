Shader "Hidden/Flow-1"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _Bias ("Bias Texture", 2D) = "black" { }
        _BiasMult ("Bias Multiply", vector) = (1, 1, 1, 1)
        _Size("Size", vector) = (1, 1, 1, 1)
        _UVSize("UvSize", vector) = (1, 1, 1, 1)
        _BiasSize("Bias Size", vector) = (1 ,1 , 1, 1)
        _FlowSpeed("Flow Speed", vector) = (1, 1, 1, 1)
        _CenteralLight("Centeral Light", float) = 1.0
        _HighLightColor("Highlight Color", color) = (1, 1, 1, 1)
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
                float4 vertex : SV_POSITION;
                float4 color : COLOR0;
            };

            sampler2D _MainTex;
            sampler2D _Bias;
            float4 _BiasMult;
            float4 _Size;
            float4 _UVSize;
            float4 _BiasSize;
            float4 _FlowSpeed;
            float _CenteralLight;
            float4 _HighLightColor;
            
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
                // 处理主颜色扭曲.
                {
                    float2 biasSamplePoint = i.uv * _BiasSize.xy + float2(0, _Time.y * _FlowSpeed.y);
                    float bias = xmap(tex2D(_Bias, float2(0, biasSamplePoint.y)).a, 0, 1, -1, 1);
                    float2 texSamplePoint = i.uv * _Size + float2(0, _Time.y * _FlowSpeed.x);
                    float alphaMult = max(0, 1 - sqr(texSamplePoint - _Size.x * 0.5) * (1 / _UVSize.x));
                    texSamplePoint.x += bias * _BiasMult.x;
                    color = tex2D(_MainTex, texSamplePoint);
                    color.a *= alphaMult;
                }
                
                float4 highlight = float4(0, 0, 0, 0);
                // 处理主颜色扭曲.
                {
                    float2 biasSamplePoint = i.uv * _BiasSize.zw + float2(0, _Time.y * _FlowSpeed.w);
                    float bias = xmap(tex2D(_Bias, float2(0, biasSamplePoint.y)).a, 0, 1, -1, 1);
                    float2 texSamplePoint = i.uv * _Size.zw + float2(0, _Time.y * _FlowSpeed.z);
                    texSamplePoint.x += bias * _BiasMult.z;
                    float alphaMult = max(0, 1 - sqr(texSamplePoint - _Size.z * 0.5) * (1 / _UVSize.z));
                    highlight = tex2D(_MainTex, texSamplePoint);
                    highlight.a *= alphaMult;
                }
                
                return float4(color.rgb * i.color.rgb + highlight.a * _HighLightColor.rgb, 
                    color.a * i.color.a + highlight.a * _HighLightColor.a);
            }
            ENDCG
        }
    }
}
