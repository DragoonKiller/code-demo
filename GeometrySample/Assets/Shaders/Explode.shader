Shader "Hidden/Explode"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" { }
        _Scale("Scale", vector) = (1, 1, 1, 1)
        _EnvColor("Environment Color", vector) = (1, 1, 1, 1)
        _ReflectionRate("Reflection Rate", float) = 1.0
        _WindScale("Wind Scale", float) = 1.0
        _WindSpeed("Wind Speed", float) = 1.0
        _WindWaveLength("Wind Wave Length", float) = 1.0
        _WindDir("Wind Direction", vector) = (1, 0, 0, 0)
    }
        SubShader
    {
            Tags{"LightMode" = "ForwardBase"}

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geo
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "Util.cginc"

            struct appdata
            {
                float4 normal : NORMAL0;
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
            };

            struct v2g
            {
                float4 normal : NORMAL0;
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION0;
            };

            struct g2f
            {
                float4 normal : NORMAL0;
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : POSITION1;
                float4 objectPos : POSITION2;
            };

            sampler2D _MainTex;
            float4 _Scale;
            float4 _EnvColor;
            float _ReflectionRate;
            float _WindScale;
            float _WindWaveLength;
            float _WindSpeed;
            float4 _WindDir;

            v2g vert(appdata v)
            {
                v2g o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                o.normal = v.normal;
                return o;
            }

            void submit(inout TriangleStream<g2f> trs, float3 vert, float2 uv, float3 normal)
            {
                g2f o;
                o.vertex = UnityObjectToClipPos(vert);
                o.worldPos = mul(unity_ObjectToWorld, vert);
                o.objectPos = float4(vert, 1.0);
                o.uv = uv;
                o.normal = float4(normal, 0.0);
                trs.Append(o);
            }

            void submitTriangle(
                inout TriangleStream<g2f> trs, 
                float3 vert1, float2 uv1, float3 normal1,
                float3 vert2, float2 uv2, float3 normal2,
                float3 vert3, float2 uv3, float3 normal3
            )
            {
                submit(trs, vert1, uv1, normal1);
                submit(trs, vert2, uv2, normal2);
                submit(trs, vert3, uv3, normal3);
                trs.RestartStrip();
            }

            [maxvertexcount(24)]
            void geo(triangle v2g input[3], inout TriangleStream<g2f> trs)
            {
                float4 vs[3];
                vs[0] = input[0].vertex;
                vs[1] = input[1].vertex;
                vs[2] = input[2].vertex;

                float3 normal[4];
                normal[3] = 0.33333333 * (input[0].normal + input[1].normal + input[2].normal);
                normal[0] = cross(normal[3], vs[1] - vs[0]);
                normal[1] = cross(normal[3], vs[2] - vs[1]);
                normal[2] = cross(normal[3], vs[0] - vs[2]);
                
                float3 extrude = normal[3] * _Scale[3];

                float3 ts[3];
                ts[0] = vs[0] + extrude;
                ts[1] = vs[1] + extrude;
                ts[2] = vs[2] + extrude;

                float3 center = 0.33333333 * (ts[0] + ts[1] + ts[2]);
                // 加一个随风飘动的特效.
                float3 worldRef = mul(unity_ObjectToWorld, center);
                center += sin((_Time.y * _WindSpeed + worldRef) / _WindWaveLength) * _WindScale * _WindDir.xyz;

                ts[0] = (ts[0] - center) * 0.5 + center;
                ts[1] = (ts[1] - center) * 0.5 + center;
                ts[2] = (ts[2] - center) * 0.5 + center;

                float2 uv[3];
                uv[0] = input[0].uv;
                uv[1] = input[1].uv;
                uv[2] = input[2].uv;

                submitTriangle(trs,
                    ts[0], uv[0], normal[3],
                    ts[1], uv[1], normal[3],
                    ts[2], uv[2], normal[3]
                );

                submitTriangle(trs,
                    ts[0], uv[2], normal[0],
                    vs[0], uv[0], normal[0],
                    vs[1], uv[1], normal[0]
                );
                submitTriangle(trs,
                    ts[0], uv[2], normal[0],
                    vs[1], uv[0], normal[0],
                    ts[1], uv[1], normal[0]
                );

                submitTriangle(trs,
                    ts[1], uv[2], normal[1],
                    vs[1], uv[0], normal[1],
                    vs[2], uv[1], normal[1]
                );
                submitTriangle(trs,
                    ts[1], uv[2], normal[1],
                    vs[2], uv[0], normal[1],
                    ts[2], uv[1], normal[1]
                );

                submitTriangle(trs,
                    ts[2], uv[2], normal[2],
                    vs[2], uv[0], normal[2],
                    vs[0], uv[1], normal[2]
                );
                submitTriangle(trs,
                    ts[2], uv[2], normal[2],
                    vs[0], uv[0], normal[2],
                    ts[0], uv[1], normal[2]
                );

            }


            fixed4 frag(g2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(WorldSpaceViewDir(i.objectPos));
                float3 normalDir = normalize(mul(unity_ObjectToWorld, float4(i.normal.xyz, 1.0))).xyz;
                
                // 环境光基础色 + Lambert 方向光漫反射.
                float4 illu = _EnvColor + _LightColor0 * col * max(0, dot(lightDir, normalDir));

                // Blinn 高光反射.
                float reflectionIllu = dot(2 * dot(lightDir, normalDir) * normalDir - lightDir, viewDir);
                float4 illx = max(0, _LightColor0 * pow(reflectionIllu, _ReflectionRate));

                return illu + illx;
            }
            ENDCG
        }
    }
}
