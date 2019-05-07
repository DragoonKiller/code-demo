// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Hidden/BackgroundShader"
{
    Properties
    {
        // cannot declare constant outside.
        pi("pi", float) = 0.0
        
        // Random seed.
        rdSeed("rdSeed", float) = 0.0
        
        // the resolution of render target.
        resolution("resolution", vector) = (0, 0, 0, 0) 
        
        // Should be Size / Resolution.,
        //   where Size is chunk size in world space,
        //   and Resolution is target texture's texel size.
        localScale("scale", vector) = (0, 0, 0, 0)
        
        freq("freq", vector) = (0, 0, 0, 0)
        
        // Roughness of the ground.
        roughness("roughness", float) = 0.0
        
        // For all y >= this value, caves will be shrinked.
        caveShrinkY("caveShrinkY", float) = 0.0
        
        // For all y >= this value, caves will be eliminated.
        caveEliminateY("caveEliminateY", float) = 0.0
        
        // the scale along two axis.
        amplitude("amplitude", vector) = (0, 0, 0, 0)
        
        // The color of empty background.
        emptyColor("emptyColor", vector) = (0, 0, 0, 0)
        
        // The color of filled background.
        filledColor("filledColor", vector) = (0, 0, 0, 0)
    }
    
    SubShader
    {
        Cull Off
        ZWrite Off
        ZTest Always

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
                float2 pos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float pi;
            float rdSeed;
            float2 resolution;
            float2 localScale;
            float2 freq;
            float roughness;
            float caveShrinkY;
            float caveEliminateY;
            float2 amplitude;
            float4 emptyColor;
            float4 filledColor;
            
            float rdi(float x, float y)
            {
                float dx = x * 681113.14556871011 + rdSeed * 1928529.5237881;
                float dy = y * 123115.12339128301 + rdSeed * 1438983.1112232;
                int g = int(dx + dy);
                g = g ^ (g << 3);
                g = g * 12577;
                g = g ^ (g << 4);
                float res = (g / 2147483647.0) * 0.5 + 0.5;
                return res;
            }
            
            float sqr(float x) { return x * x; }
            float plp(float x) { return 6 * pow(x, 5) - 15 * pow(x, 4) + 10 * pow(x, 3); }
            float2 rot(float2 v, float a) { return float2( v.x * cos(a) - v.y * sin(a), v.x * sin(a) + v.y * cos(a) ); }
            float xmap(float l, float r, float a, float b, float x) { return (x - l) / (r - l) * (b - a) + a; }

            float2 sampleVec(float2 s)
            {
                float2 dir = rot(float2(1, 0), xmap(0.0, 1.0, 0.0, 2.0 * pi, rdi(s.x, s.y)));
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
                float scale = 2.12;
                float res[4] = { 0.0, 0.0, 0.0, 0.0 };
                for(int i=0; i<4; i++) res[i] = gradInt(pos, base[i]) * scale;
                
                return itp(
                        itp(res[0], res[1], pos.x - floor(pos.x)),
                        itp(res[2], res[3], pos.x - floor(pos.x)),
                        pos.y - floor(pos.y)
                );
            }

            float perlin(float a, float b, float2 freq, float2 uv) { return xmap(-1.0, 1.0, a, b, coherent(freq, uv)); }
            float ridge(float a, float b, float2 freq, float2 uv) { return xmap(0.0, 1.0, a, b, sqr(1.0 - abs(coherent(freq, uv)))); }
            float2 perlin2(float a, float b, float2 freq, float2 uv)
            {
                return float2(
                    perlin(a, b, freq, float2(132.1123, uv.x)),
                    perlin(a, b, freq, float2(uv.y, 132.1123))
                );
            }

            float illu(float2 uv)
            {
                float2 vibrate = perlin2(-roughness, roughness, freq * 100, uv);
                
                float ground = (
                    // the sea level.
                    uv.y
                    // The mountain and hills.
                    + perlin(-2, 2, freq, float2(uv.x, 1172.58529)) * amplitude.y
                    // ridges.
                    + ridge(-1, 1, freq, float2(uv.x, 7739.11827) + vibrate) * amplitude.y
                    - ridge(-1, 1, freq, float2(uv.x, 6537.73329) + vibrate) * amplitude.y
                    // small stones.
                    + perlin(-0.2, 0.2, freq * 10, float2(uv.x, 1624.33321)) * amplitude.y
                    + perlin(-0.1, 0.1, freq * 20, float2(uv.x, 1225.63424)) * amplitude.y
                ) > 0;
                
                float caveShrink = caveShrinkY * amplitude.y;
                float caveEliminate = caveEliminateY * amplitude.y;
                float caveY = uv.y + perlin(-amplitude.y, amplitude.y, freq, float2(uv.x, 1739.88442));
                float cave = (
                    0.0
                    // the cave shape.
                    + perlin(-0.5, 1.0, freq * 2, uv + vibrate)
                    + perlin(-1.0, 0.4, freq * 10, uv + vibrate)
                    + perlin(-0.5, 0.1, freq * 20, uv + vibrate)
                    // Shrink or eliminate the cave if it is over boundary.
                    - ( caveShrink < caveY && caveY < caveEliminate ? (caveY - caveShrink) / (caveEliminate - caveShrink)
                        : caveShrink <= caveY ? 1.0
                        : 0.0
                    )
                ) > 0;
                
                return max(ground, cave);
            }
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.pos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                float il = illu(i.pos);
                float4 color = emptyColor + (filledColor - emptyColor) * (1 - il);
                return color;
            }
            ENDCG
        }
    }
}
