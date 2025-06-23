Shader "Custom/BorderDotEffect"
 {
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1 ("Color 1", Color) = (0.5, 0, 1, 1) // Tím
        _Color2 ("Color 2", Color) = (0, 1, 0.5, 1) // Xanh
        _Color3 ("Color 3", Color) = (1, 0, 0, 1) // Đỏ
        _Speed ("Speed", Float) = 1.0 // Tốc độ di chuyển
        _Width ("Border Width", Float) = 0.1 // Độ rộng viền
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
            float _Width;

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
                fixed4 texCol = tex2D(_MainTex, i.uv);
                // Chỉ xử lý phần viền (alpha > 0)
                if (texCol.a == 0) return fixed4(0, 0, 0, 0);

                // Tính góc từ UV (tâm là 0.5, 0.5)
                float angle = atan2(i.uv.y - 0.5, i.uv.x - 0.5) / (2 * 3.14159) + 0.5;
                // Tạo gradient di chuyển dựa trên thời gian
                float gradientPos = frac(angle + _Time.y * _Speed);

                // Tạo gradient màu
                fixed4 finalColor;
                if (gradientPos < 0.33) {
                    finalColor = lerp(_Color1, _Color2, gradientPos / 0.33);
                } else if (gradientPos < 0.66) {
                    finalColor = lerp(_Color2, _Color3, (gradientPos - 0.33) / 0.33);
                } else {
                    finalColor = lerp(_Color3, _Color1, (gradientPos - 0.66) / 0.33);
                }

                // Áp dụng độ rộng viền
                float dist = max(abs(i.uv.x - 0.5), abs(i.uv.y - 0.5));
                float alpha = smoothstep(0.5, 0.5 - _Width, dist);
                finalColor.a = texCol.a * alpha;

                return finalColor;
            }
            ENDCG
        }
    }
}