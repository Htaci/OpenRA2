Shader "UI/GlassEffect"
{
    Properties
    {
        _BlurAmount("Blur Amount", Range(0, 20)) = 5
        _Transparency("Transparency", Range(0, 1)) = 0.5
        _GlassColor("Glass Color", Color) = (1,1,1,1)
        _NoiseScale("Noise Scale", Range(0, 1)) = 0.1
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent" 
            "IgnoreProjector" = "True" 
            "RenderType" = "Transparent"
        }
        
        GrabPass
        {
            "_BackgroundTexture"
        }
        
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
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD1;
            };
            
            sampler2D _BackgroundTexture;
            float _BlurAmount;
            float _Transparency;
            float4 _GlassColor;
            float _NoiseScale;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                o.uv = v.uv;
                return o;
            }
            
            // 修正的随机噪声函数
            float rand(float2 co) {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }
            
            // 高斯模糊采样
            float4 blurSample(sampler2D tex, float2 uv, float blur)
            {
                float4 col = float4(0,0,0,0);
                float total = 0.0;
                float offset = blur / 1000.0;
                
                // 3x3高斯模糊
                for (float x = -1.0; x <= 1.0; x += 1.0)
                {
                    for (float y = -1.0; y <= 1.0; y += 1.0)
                    {
                        float weight = exp(-(x*x + y*y) / (2.0));
                        col += tex2D(tex, uv + float2(x, y) * offset) * weight;
                        total += weight;
                    }
                }
                
                return col / total;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // 计算屏幕UV
                float2 screenUV = i.grabPos.xy / i.grabPos.w;
                
                // 应用模糊效果
                float4 blurredColor = blurSample(_BackgroundTexture, screenUV, _BlurAmount);
                
                // 添加磨砂噪声效果
                float noise = rand(i.uv * _Time.y * _NoiseScale);
                blurredColor.rgb += (noise - 0.5) * 0.05;
                
                // 应用玻璃颜色和透明度
                blurredColor.rgb = lerp(blurredColor.rgb, _GlassColor.rgb, _GlassColor.a);
                blurredColor.a = _Transparency;
                
                return blurredColor;
            }
            ENDCG
        }
    }
}