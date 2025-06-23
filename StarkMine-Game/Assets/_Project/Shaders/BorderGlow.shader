Shader "UI/BorderGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BorderWidth ("Border Width", Range(0.01, 0.5)) = 0.1
        _Speed ("Glow Speed", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue"="Transparent"
        }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _BorderWidth;
            float _Speed;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float3 HSVtoRGB(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 uv = i.uv;

                float dist = distance(uv, center);
                float border = smoothstep(0.5, 0.5 - _BorderWidth, dist);

                // Tạo hiệu ứng màu chạy quanh theo góc (polar coordinate)
                float angle = atan2(uv.y - 0.5, uv.x - 0.5);
                float t = fmod(_Time.x * _Speed + angle / (2 * 3.14159), 1.0);
                float3 glowColor = HSVtoRGB(float3(t, 1.0, 1.0));

                return float4(glowColor, border);
            }
            ENDCG
        }
    }
}