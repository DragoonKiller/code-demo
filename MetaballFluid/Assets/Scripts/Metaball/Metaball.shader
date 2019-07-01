Shader "Hidden/Metaball"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" { }
        _Color("Color", vector) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend One OneMinusSrcAlpha
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
            float4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // Since alpha channel is not usable,
                return col * _Color;
            }
            ENDCG
        }
    }
}
