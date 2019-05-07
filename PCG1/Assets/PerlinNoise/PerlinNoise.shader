Shader "Hidden/PerlinNoise"
{
    Properties
    {
        pi("PI", float) = 3.141592653589793
        rdSeed("RandomSeed", float) = 1.0
        grSize("GridSize", vector) = (1.0, 1.0, 1.0, 1.0)
        fixParam("FixParam", float) = 1.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        
        Pass
        {
            CGPROGRAM
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float pi; // cannot declare constant outside.
            float rdSeed;
            float4 rdSize;
            float4 grSize;
            float fixParam;
            
            float rdi(float x, float y)
            {
                x *= 1.14556871011 + rdSeed * 1.5237881;
                y *= 2553.39128301 + rdSeed * 43.112232;
                uint g = uint(x + y);
                g = g ^ (g << 3);
                g = g * (1225);
                g = g ^ (g << 4);
                g = g * (131697);
                g = g ^ (g << 1);
                g = g + rdSeed * 431.0;
                g = g ^ (g >> 1);
                return clamp(float(g) / 4294967296.0, 0, 1);
            }
            
            float sqr(float x) { return x * x; }
            float cosp(float x) { return (1.0 - cos(x * pi)) * 0.5; }
            float sinp(float x) { return sin(x * 0.5 * pi); }
            float circup(float x) { return 1.0 - sqrt(1.0 - x * x); }
            float circdp(float x) { return sqrt(1.0 - sqr(1.0 - x)); }
            float plp(float x) { return 6 * pow(x, 5.0) - 15 * pow(x, 4.0) + 10 * pow(x, 3.0); }
            float polyxp(float x) { return x <= 0.5 ? 2 * sqr(x) : 1.0 - 2.0 * sqr(1 - x); }
            float plsign(float x) { return x < -0.3333333 ? -1 : x > 0.3333333 ? 1 : 0; }
            float2 rot(float2 v, float a) { return float2( v.x * cos(a) - v.y * sin(a), v.x * sin(a) + v.y * cos(a) ); }
            float xmap(float l, float r, float a, float b, float x) { return (x - l) / (r - l) * (b - a) + a; }
            
            float2 sampleVec(float2 s)
            {
                float2 dir = rot(float2(1, 0), xmap(0, 1, 0, 2.0 * pi, rdi(s.x, s.y)));
                dir.x = plsign(dir.x);
                dir.y = plsign(dir.y);
                // float2 dir = float2(
                //     xmap(0, 1, -1, 1, rdi(s.x, s.y)),
                //     xmap(0, 1, -1, 1, rdi(s.x * 3.335532, s.y * 2.667529))
                // );
                return dir;
            }
            
            float gradInt(float2 c, float2 s) { return dot(c - s, sampleVec(s)); }
            float itp(float l, float r, float x) { return l + (r - l) * plp(x); }
            float coherent(float2 freq, float2 uv)
            {
                float2 pos = uv * freq;
                
                const float upx = 0.999999;
                float2 base[4] = {
                    floor(pos),
                    floor(pos + float2(upx, 0)),
                    floor(pos + float2(0, upx)),
                    floor(pos + float2(upx, upx)),
                };
                
                // TODO: Determine how this constant comes up.
                //   It is to do with the interpolation method.
                const float scale = 2.12;
                float res[4] = { 0.0, 0.0, 0.0, 0.0 };
                for(int i=0; i<4; i++) res[i] = gradInt(pos, base[i]) * scale;
                
                return itp(
                        itp(res[0], res[1], pos.x - floor(pos.x)),
                        itp(res[2], res[3], pos.x - floor(pos.x)),
                        pos.y - floor(pos.y)
                );
            }
            
            float perlin(float2 freq, float2 uv) { return xmap(-1, 1, 0, 1, coherent(freq, uv)); }
            float ridge(float2 freq, float2 uv) { return sqr(1.0 - abs(coherent(freq, uv))); }
            
            float4 frag(v2f i) : SV_Target
            {
                float ground = (
                    // basic height part.
                    i.uv.y + 
                    // low freq height accumulate part.
                    perlin(grSize.xy * 0.1, float2(1130.03, i.uv.x)) * 0.25 +
                    ridge(grSize.xy * 0.2, float2(1130.03, i.uv.x)) * 0.05 +
                    ridge(grSize.xy * 0.4, float2(30.03, i.uv.x)) * 0.01 +
                    // surface dithering part.
                    (perlin(grSize.xy * 0.1, i.uv) * 0.02 - 0.01) +
                    (perlin(grSize.xy, i.uv) * 0.005 - 0.0025) +
                    // high freq height accumulate part.
                    (perlin(grSize.xy * 2.0, i.uv) * 0.0001 - 0.00005) +
                    (perlin(grSize.xy * 4.0, i.uv) * 0.0002 - 0.0001) + 
                    (perlin(grSize.xy * 8.0, i.uv) * 0.0004 - 0.0002) +
                    (perlin(grSize.xy * 16.0, i.uv) * 0.0002 - 0.0001) +
                    (perlin(grSize.xy * 32.0, i.uv) * 0.0001 - 0.00005)
                ) > 0.5;
                
                float caves = (
                    ridge(
                        grSize.xy * 0.1,
                        i.uv +
                        (perlin(grSize.xy * 0.2, i.uv) * 0.02 - 0.01) +
                        (perlin(grSize.xy * 0.4, i.uv) * 0.01 - 0.005)
                    ) +
                    ridge(grSize.xy * 0.05, float2(1130.03, i.uv.x)) * 0.2 - 0.1 +
                    i.uv.y - 1.0
                ) > 0.5;
                
                ground = ground - caves;
                
                return float4(ground, ground, ground, 1);
            }
            
            ENDCG
        }
    }
}
