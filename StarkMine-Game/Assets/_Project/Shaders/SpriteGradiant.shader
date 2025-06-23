Shader "Custom/VerticalGradientMask"
{
    Properties
    {
        [PerRendererData] _MainTex ("Mask (Alpha)", 2D) = "white" {}
        _Color1 ("Top Color", Color) = (1,0,0,1)
        _Color2 ("Bottom Color", Color) = (0,0,1,1)
        _HighlightStrength ("Highlight Strength", Range(0,1)) = 0.3
        _HighlightRadius ("Highlight Radius", Range(0,1)) = 0.2
        
        // Thêm các thuộc tính stencil
        [PerRendererData] _Stencil ("Stencil ID", Float) = 0
        _StencilComp ("Stencil Comparison", Float) = 8
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        LOD 100

        // Thêm cấu hình stencil
        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ColorMask [_ColorMask]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color1;
            fixed4 _Color2;
            float _HighlightStrength;
            float _HighlightRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the alpha from the mask
                fixed4 mask = tex2D(_MainTex, i.uv);
                // Compute a vertical gradient based on UV.y (0 = bottom, 1 = top)
                float gradient = i.uv.y;

                // Lerp from _Color2 (bottom) to _Color1 (top).
                fixed4 col = lerp(_Color2, _Color1, gradient);
                float2 center = float2(0.5, 1.0);
                float dist = distance(i.uv, center);

                // If dist < _HighlightRadius, add some brightness
                float highlight = 1.0 - saturate(dist / _HighlightRadius);
                col.rgb += highlight * _HighlightStrength;

                // Apply the mask's alpha so only the silhouette is visible.
                col.a *= mask.a;
                return col;
            }
            ENDCG
        }
    }
}