Shader "Custom/BorderColorShift"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (1, 0, 0, 1) // Màu đầu tiên
        _Color2 ("Color 2", Color) = (0, 1, 0, 1) // Màu thứ hai
        _Color3 ("Color 3", Color) = (0, 0, 1, 1) // Màu thứ ba
        _Speed ("Speed", Float) = 1.0 // Tốc độ đổi màu
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

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
            float4 _Color1;
            float4 _Color2;
            float4 _Color3;
            float _Speed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Lấy texture của viền
                fixed4 col = tex2D(_MainTex, i.uv);
                // Tính toán màu đổi dần theo thời gian
                float t = (_Time.y * _Speed) % 3;
                fixed4 finalColor;
                if (t < 1) {
                    finalColor = lerp(_Color1, _Color2, t);
                } else if (t < 2) {
                    finalColor = lerp(_Color2, _Color3, t - 1);
                } else {
                    finalColor = lerp(_Color3, _Color1, t - 2);
                }
                // Chỉ áp dụng màu cho phần viền (alpha > 0)
                return col.a > 0 ? finalColor * col.a : col;
            }
            ENDCG
        }
    }
}