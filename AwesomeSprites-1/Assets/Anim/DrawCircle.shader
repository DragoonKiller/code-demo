Shader "Hidden/DrawCircle"
{
    Properties
    {
        _DrawRect("Draw Rect (world space)", vector) = (0, 0, 1, 1)
        _InColor("Color", vector) = (1, 1, 1, 1)
        _OutColor("Color", vector) = (1, 1, 1, 1)
        _InRadius("Radius (world space)", float) = 1
        _OutRadius("Radius (world space)", float) = 1
        _Center("Center (world space)", vector) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderQueue" = "Transparent" }

        Pass
        {
            Cull Off
            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha

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
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 _DrawRect;
            float4 _InColor;
            float4 _OutColor;
            float _InRadius;
            float _OutRadius;
            float2 _Center;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 worldPos = _DrawRect.xy + (_DrawRect.zw - _DrawRect.xy) * i.uv;
                float dist = length(worldPos - _Center);
                if(dist < _InRadius || dist > _OutRadius) discard;
                float rate = (dist - _InRadius) / (_OutRadius - _InRadius);

                return rate * _OutColor + (1 - rate) * _InColor;
            }
            ENDCG
        }
    }
}
