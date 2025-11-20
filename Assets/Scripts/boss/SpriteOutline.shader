Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float4 _Color;
            float4 _OutlineColor;
            float _OutlineSize;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
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

            float4 frag (v2f i) : SV_Target
            {
                // Sample original
                float4 pixel = tex2D(_MainTex, i.uv);
                
                // If fully transparent, check neighbors
                if (pixel.a == 0)
                {
                    float2 pixelOffset = _MainTex_TexelSize.xy * _OutlineSize;

                    float4 up    = tex2D(_MainTex, i.uv + float2(0, pixelOffset.y));
                    float4 down  = tex2D(_MainTex, i.uv - float2(0, pixelOffset.y));
                    float4 left  = tex2D(_MainTex, i.uv - float2(pixelOffset.x, 0));
                    float4 right = tex2D(_MainTex, i.uv + float2(pixelOffset.x, 0));

                    if (up.a > 0 || down.a > 0 || left.a > 0 || right.a > 0)
                        return _OutlineColor;
                }

                return pixel * _Color;
            }
            ENDCG
        }
    }
}
