Shader "UI/Window"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // 圆角参数
        _Radius ("Corner Radius", Range(0, 0.5)) = 0.1
        // 描边参数
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.02
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        // 圆角平滑度
        _Smoothness ("Smoothness", Range(0, 0.1)) = 0.01
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 uv       : TEXCOORD1;
                float4 worldPosition : TEXCOORD2;
            };
            
            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _Radius;
            float _OutlineWidth;
            float _Smoothness;
            float4 _ClipRect;
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                OUT.uv = v.texcoord;
                OUT.color = v.color * _Color;
                return OUT;
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                // 基础纹理采样
                half4 color = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // 计算圆角
                float2 uv = IN.uv;
                uv = uv * 2.0 - 1.0; // 转换到[-1,1]范围
                
                // 计算四个角的距离
                float2 cornerDist = abs(uv);
                float2 cornerVector = max(cornerDist - (1.0 - _Radius), 0.0);
                float cornerDistance = length(cornerVector);
                
                // 圆角裁剪
                float roundFactor = 1.0 - smoothstep(
                    _Radius - _Smoothness, 
                    _Radius + _Smoothness, 
                    cornerDistance
                );
                
                // 描边计算
                float outlineFactor = smoothstep(
                    _Radius + _OutlineWidth - _Smoothness,
                    _Radius + _OutlineWidth + _Smoothness,
                    cornerDistance
                );
                
                // 应用描边
                float4 outline = _OutlineColor * (1.0 - outlineFactor);
                outline.a *= color.a;
                
                // 组合效果
                color = color * roundFactor + outline;
                color.a *= roundFactor; // 应用圆角透明度
                
                // 应用UI裁剪
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                
                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif
                
                return color;
            }
            ENDCG
        }
    }
}