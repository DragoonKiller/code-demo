Shader "Hidden/Centralize"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" { }
        _Bias("Bias Texture", 2D) = "white" { }
        _AbsorbSpeed("Absorb Speed", float) = 1.0
        _Trumble("Trumble", vector) = (1, 1, 1, 1)
        _BlackHoleRadius("Black Hole Radius", float) = 0.1
        _SampleScale("Sample Scale", float) = 1.0
        _LightMult("Light Mult", vector) = (1, 1, 1, 1)
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
            #pragma geometry geo
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Assets/Utils/utils.cginc"


            // L 后缀: 物体局部坐标系 (_) (默认)
            // W 后缀: 世界坐标系 (M)
            // V 后缀: 相机坐标系 (MV)
            // N 后缀: 裁剪视窗坐标系 (MVP)
            // F 后缀: 屏幕像素坐标系

            struct appdata
            {
                float4 posL : POSITION0;
                float2 uv : TEXCOORD0;
                float4 color : COLOR0;
            };

            struct v2g
            {
                float2 uv : TEXCOORD0;
                float4 posL : POSITION0;
                float4 color : COLOR0;
            };

            struct g2f
            {
                float2 uv : TEXCOORD0;
                float4 posN : POSITION0;
                float4 color : COLOR0;
            };

            sampler2D _MainTex;
            sampler2D _Bias;
            float _AbsorbSpeed;
            float4 _Trumble;
            float _SampleScale;
            float _BlackHoleRadius;
            float4 _LightMult;

            v2g vert(appdata v)
            {
                v2g o;
                o.posL = v.posL;
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            [maxvertexcount(4)]
            void geo(in triangle v2g inp[3], inout TriangleStream<g2f> trs)
            {
                float4 a = inp[0].posL;
                float4 b = inp[1].posL;
                float4 c = inp[2].posL;
                float3 center = float3(0, 0, 0);
                if (abs(dot(a - c, a - b)) < 1e-4) center = (b + c) / 2;
                if (abs(dot(b - a, b - c)) < 1e-4) center = (a + c) / 2;
                if (abs(dot(c - a, c - b)) < 1e-4) center = (a + b) / 2;
                float2 uniCenter = float2(0.5, 0.5);
                
                g2f o;

                o.posN = UnityObjectToClipPos(a);
                o.uv = uniCenter + (inp[0].uv - uniCenter) * 2 - float2(0.5, 0.5);
                o.color = inp[0].color;
                trs.Append(o);
                
                o.posN = UnityObjectToClipPos(b);
                o.uv = uniCenter + (inp[1].uv - uniCenter) * 2 - float2(0.5, 0.5);
                o.color = inp[1].color;
                trs.Append(o);
                
                o.posN = UnityObjectToClipPos(c);
                o.uv = uniCenter + (inp[2].uv - uniCenter) * 2 - float2(0.5, 0.5);
                o.color = inp[2].color;
                trs.Append(o);
                
                trs.RestartStrip();
            }

            fixed4 frag(g2f i) : SV_Target
            {
                float pi = 3.141592653589793;
                float2 uv = i.uv;
                float radius = sqrt(uv.x * uv.x + uv.y * uv.y);
                float dir = atan2(uv.y, uv.x);
                float2 samplePoint = float2(dir / pi, _SampleScale * pow(radius, 1.5) + _Time.x * _AbsorbSpeed);
                float2 bias = float2(
                    tex2D(_Bias, samplePoint).a - 0.5,
                    tex2D(_Bias, float2(samplePoint.y, samplePoint.x)).a - 0.5
                ) * 2.0 * _Trumble;
                float4 sampledColor = tex2D(_MainTex, samplePoint + bias);
                float alphaSample = 0.0;
                if (radius > _BlackHoleRadius)
                {
                    float x = (radius - _BlackHoleRadius) / (1.0 - _BlackHoleRadius);
                    x = 1 - x;
                    alphaSample = x / exp(x - 1);
                }
                else
                {
                    float x = radius / _BlackHoleRadius;
                    alphaSample = x / exp(x - 1);
                }
                float4 alphaMult = float4(1, 1, 1, alphaSample);
                float baseLight = xmap(radius, 0, 1, _LightMult.y, _LightMult.x);
                return (sampledColor * i.color + baseLight) * alphaMult;
            }
            ENDCG
        }
    }
}
