Shader "UI/7ColorMaskShader_Final"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture (is Mask)", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        
        [Header(Region Colors)]
        _ColorR("Color (R)", Color) = (1,0,0,1)
        _ColorG("Color (G)", Color) = (0,1,0,1)
        _ColorB("Color (B)", Color) = (0,0,1,1)
        _ColorRG("Color (R+G)", Color) = (1,1,0,1)
        _ColorRB("Color (R+B)", Color) = (1,0,1,1)
        _ColorGB("Color (G+B)", Color) = (0,1,1,1)
        _ColorRGB("Color (R+G+B)", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        ZWrite Off 
        Blend SrcAlpha OneMinusSrcAlpha 
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _ColorR, _ColorG, _ColorB, _ColorRG, _ColorRB, _ColorGB, _ColorRGB;

            struct v2f { float4 vertex : SV_POSITION; fixed4 color : COLOR; float2 texcoord : TEXCOORD0; };
            struct appdata { float4 vertex : POSITION; float4 color : COLOR; float2 texcoord : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 maskColor = tex2D(_MainTex, i.texcoord);
                fixed4 finalColor = fixed4(0, 0, 0, 0);
                float threshold = 0.8;

                if (maskColor.r > threshold && maskColor.g > threshold && maskColor.b > threshold) { finalColor = _ColorRGB; }
                else if (maskColor.r > threshold && maskColor.g > threshold) { finalColor = _ColorRG; }
                else if (maskColor.r > threshold && maskColor.b > threshold) { finalColor = _ColorRB; }
                else if (maskColor.g > threshold && maskColor.b > threshold) { finalColor = _ColorGB; }
                else if (maskColor.r > threshold) { finalColor = _ColorR; }
                else if (maskColor.g > threshold) { finalColor = _ColorG; }
                else if (maskColor.b > threshold) { finalColor = _ColorB; }
                
                finalColor.rgb *= i.color.rgb;
                finalColor.a *= maskColor.a * i.color.a;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}