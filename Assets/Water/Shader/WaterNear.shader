// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33065,y:32663,varname:node_2865,prsc:2|emission-2413-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:5288,x:31948,y:32730,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:node_5288,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ef6b3d0bf823c2641bdbacb033d8894f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2275,x:32229,y:32619,varname:node_2275,prsc:2,tex:ef6b3d0bf823c2641bdbacb033d8894f,ntxv:0,isnm:False|UVIN-8657-UVOUT,TEX-5288-TEX;n:type:ShaderForge.SFN_Tex2d,id:1162,x:32226,y:32859,varname:node_1162,prsc:2,tex:ef6b3d0bf823c2641bdbacb033d8894f,ntxv:0,isnm:False|UVIN-9033-UVOUT,TEX-5288-TEX;n:type:ShaderForge.SFN_Add,id:9005,x:32479,y:32718,varname:node_9005,prsc:2|A-2275-RGB,B-1162-RGB;n:type:ShaderForge.SFN_Panner,id:8657,x:31828,y:32564,varname:node_8657,prsc:2,spu:-0.5,spv:0.4|UVIN-4394-UVOUT,DIST-4138-OUT;n:type:ShaderForge.SFN_Panner,id:9033,x:31816,y:32910,varname:node_9033,prsc:2,spu:0.25,spv:-0.25|UVIN-4394-UVOUT,DIST-4138-OUT;n:type:ShaderForge.SFN_TexCoord,id:4394,x:31486,y:32638,varname:node_4394,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Slider,id:3899,x:31312,y:33019,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_3899,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.07972731,max:1;n:type:ShaderForge.SFN_Time,id:5338,x:31386,y:33280,varname:node_5338,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4138,x:31650,y:33024,varname:node_4138,prsc:2|A-3899-OUT,B-5338-TDB;n:type:ShaderForge.SFN_SceneColor,id:8003,x:32537,y:33317,varname:node_8003,prsc:2|UVIN-393-OUT;n:type:ShaderForge.SFN_Add,id:6725,x:32668,y:32854,varname:node_6725,prsc:2|A-9005-OUT,B-8003-RGB;n:type:ShaderForge.SFN_Divide,id:2413,x:32864,y:32883,varname:node_2413,prsc:2|A-6725-OUT,B-3187-OUT;n:type:ShaderForge.SFN_Vector1,id:3187,x:32678,y:33041,varname:node_3187,prsc:2,v1:2;n:type:ShaderForge.SFN_ScreenPos,id:7092,x:32033,y:33420,varname:node_7092,prsc:2,sctp:2;n:type:ShaderForge.SFN_Add,id:393,x:32320,y:33338,varname:node_393,prsc:2|A-7563-OUT,B-7092-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:7299,x:32433,y:32997,varname:node_7299,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-9005-OUT;n:type:ShaderForge.SFN_Multiply,id:7563,x:32099,y:33121,varname:node_7563,prsc:2|A-7299-OUT,B-2900-OUT;n:type:ShaderForge.SFN_Slider,id:2900,x:31673,y:33255,ptovrint:False,ptlb:Refraction,ptin:_Refraction,varname:node_2900,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.09068102,max:1;proporder:5288-3899-2900;pass:END;sub:END;*/

Shader "Shader Forge/WaterNear" {
    Properties {
        _Noise ("Noise", 2D) = "white" {}
        _Speed ("Speed", Range(0, 1)) = 0.07972731
        _Refraction ("Refraction", Range(0, 1)) = 0.09068102
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Speed;
            uniform float _Refraction;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 projPos : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
////// Emissive:
                float4 node_5338 = _Time;
                float node_4138 = (_Speed*node_5338.b);
                float2 node_8657 = (i.uv0+node_4138*float2(-0.5,0.4));
                float4 node_2275 = tex2D(_Noise,TRANSFORM_TEX(node_8657, _Noise));
                float2 node_9033 = (i.uv0+node_4138*float2(0.25,-0.25));
                float4 node_1162 = tex2D(_Noise,TRANSFORM_TEX(node_9033, _Noise));
                float3 node_9005 = (node_2275.rgb+node_1162.rgb);
                float3 emissive = ((node_9005+tex2D( _GrabTexture, ((node_9005.rg*_Refraction)+sceneUVs.rg)).rgb)/2.0);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "Meta"
            Tags {
                "LightMode"="Meta"
            }
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_META 1
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _Speed;
            uniform float _Refraction;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 projPos : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                float4 node_5338 = _Time;
                float node_4138 = (_Speed*node_5338.b);
                float2 node_8657 = (i.uv0+node_4138*float2(-0.5,0.4));
                float4 node_2275 = tex2D(_Noise,TRANSFORM_TEX(node_8657, _Noise));
                float2 node_9033 = (i.uv0+node_4138*float2(0.25,-0.25));
                float4 node_1162 = tex2D(_Noise,TRANSFORM_TEX(node_9033, _Noise));
                float3 node_9005 = (node_2275.rgb+node_1162.rgb);
                o.Emission = ((node_9005+tex2D( _GrabTexture, ((node_9005.rg*_Refraction)+sceneUVs.rg)).rgb)/2.0);
                
                float3 diffColor = float3(0,0,0);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, 0, specColor, specularMonochrome );
                o.Albedo = diffColor;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
