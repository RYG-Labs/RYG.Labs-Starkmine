Shader "Custom/DottedBorder"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BorderWidth ("Border Width", Range(0, 0.1)) = 0.02
        _DotSpeed ("Dot Speed", Range(0, 5)) = 1
        _DotSize ("Dot Size", Range(0.01, 0.1)) = 0.05
        _DotSpacing ("Dot Spacing", Range(0.1, 1.0)) = 0.2
        _DotColor ("Dot Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

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

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BorderWidth;
            float _DotSpeed;
            float _DotSize;
            float _DotSpacing;
            fixed4 _DotColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float dist = min(min(uv.x, 1.0 - uv.x), min(uv.y, 1.0 - uv.y));
                float border = smoothstep(_BorderWidth, _BorderWidth * 0.5, dist);
                float time = _Time.y * _DotSpeed;
                float t = frac((uv.x + uv.y + time) / _DotSpacing);
                float dot = smoothstep(_DotSize, _DotSize * 0.5, abs(t - 0.5) * _DotSpacing);
                float finalDot = dot * border;
                fixed4 texColor = tex2D(_MainTex, uv);
                return fixed4(lerp(texColor.rgb, _DotColor.rgb, finalDot), texColor.a);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}