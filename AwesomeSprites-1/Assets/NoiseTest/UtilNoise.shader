Shader "Hidden/UtilNoise"
{
    Properties
    {
        _NoiseType ("Noise Type", float) = 0
        _SampleScale ("Sample Scale", float) = 0
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
            #include "Assets/Utils/Utils.cginc"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION;
                float4 worldPos : POSITION1;
            };
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            float _NoiseType;
            float _SampleScale;
            fixed4 frag (v2f i) : SV_Target
            {
                float4 pos = i.worldPos / _SampleScale;
                float h = 0;
                if(_NoiseType < 1) h = noise(pos.x);
                else if(_NoiseType < 2) h = noise(pos.xy);
                else h = noise(pos.xyz);
                return float4(h, h, h, 1);
            }
            ENDCG
        }
    }
}
