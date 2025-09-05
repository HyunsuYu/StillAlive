Shader "Custom/7ColorMaskShader"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" {} 
        _MaskTex("Mask (R,G,B Combinations)", 2D) = "black" {} 
        
        [Header(Region Colors)] 
        _ColorR("Color (R)", Color) = (1,0,0,1) 
        _ColorG("Color (G)", Color) = (0,1,0,1) 
        _ColorB("Color (B)", Color) = (0,0,1,1) 
        _ColorRG("Color (R+G)", Color) = (1,1,0,1) 
        _ColorRB("Color (R+B)", Color) = (1,0,1,1) 
        _ColorGB("Color (G+B)", Color) = (0,1,1,1) 
        _ColorRGB("Color (R+G+B)", Color) = (1,1,1,1) 
    }
    SubShader
    {

        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off 
        Blend SrcAlpha OneMinusSrcAlpha

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
            sampler2D _MaskTex;
            fixed4 _ColorR, _ColorG, _ColorB, _ColorRG, _ColorRB, _ColorGB, _ColorRGB;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex); 
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);    
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainTexColor = tex2D(_MainTex, i.uv);
                fixed4 mask = tex2D(_MaskTex, i.uv);

                if (mask.r < 0.1 && mask.g < 0.1 && mask.b < 0.1)
                {
                    return fixed4(mainTexColor.rgb, 0); 
                }

                fixed4 finalColor = fixed4(0.0, 0.0, 0.0, 0.0);

                // 마스크 텍스처의 RGB 값 조합에 따라 어떤 색상을 사용할지 결정
                float threshold = 0.8; 

                if (mask.r > threshold && mask.g > threshold && mask.b > threshold)
                {
                    finalColor = _ColorRGB; // R, G, B 모두 켜졌을 때
                }
                else if (mask.r > threshold && mask.g > threshold)
                {
                    finalColor = _ColorRG;  // R, G가 켜졌을 때
                }
                else if (mask.r > threshold && mask.b > threshold)
                {
                    finalColor = _ColorRB;  // R, B가 켜졌을 때
                }
                else if (mask.g > threshold && mask.b > threshold)
                {
                    finalColor = _ColorGB;  // G, B가 켜졌을 때
                }
                else if (mask.r > threshold)
                {
                    finalColor = _ColorR;   // R만 켜졌을 때
                }
                else if (mask.g > threshold)
                {
                    finalColor = _ColorG;   // G만 켜졌을 때
                }
                else if (mask.b > threshold)
                {
                    finalColor = _ColorB;   // B만 켜졌을 때
                }
                
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit" 
}