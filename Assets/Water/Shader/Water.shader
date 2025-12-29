// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:34772,y:32876,varname:node_2865,prsc:2|diff-2867-OUT,spec-8993-OUT,gloss-8912-OUT,normal-7641-OUT,alpha-9865-OUT;n:type:ShaderForge.SFN_ScreenPos,id:8824,x:31730,y:32750,varname:node_8824,prsc:2,sctp:2;n:type:ShaderForge.SFN_SceneColor,id:7659,x:32684,y:32802,varname:node_7659,prsc:2|UVIN-6800-OUT;n:type:ShaderForge.SFN_Color,id:8181,x:33472,y:33167,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_8181,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0.2551723,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:2304,x:32570,y:33735,varname:node_2304,prsc:2,tex:ed1066aab3d29c947b2f28a0ebab2d4d,ntxv:3,isnm:True|UVIN-207-UVOUT,TEX-8696-TEX;n:type:ShaderForge.SFN_SceneDepth,id:9849,x:32310,y:32957,varname:node_9849,prsc:2;n:type:ShaderForge.SFN_Subtract,id:9136,x:32482,y:33061,varname:node_9136,prsc:2|A-9849-OUT,B-7973-OUT;n:type:ShaderForge.SFN_Depth,id:7973,x:32233,y:33096,varname:node_7973,prsc:2;n:type:ShaderForge.SFN_Clamp01,id:9865,x:33274,y:33130,varname:node_9865,prsc:2|IN-7392-OUT;n:type:ShaderForge.SFN_Lerp,id:2867,x:33965,y:32737,varname:node_2867,prsc:2|A-4393-OUT,B-8181-RGB,T-9865-OUT;n:type:ShaderForge.SFN_Slider,id:7402,x:32154,y:33253,ptovrint:False,ptlb:depth,ptin:_depth,varname:node_7402,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.660608,max:1;n:type:ShaderForge.SFN_Tex2d,id:9746,x:32250,y:32252,varname:node_9746,prsc:2,tex:ef6b3d0bf823c2641bdbacb033d8894f,ntxv:2,isnm:False|UVIN-274-UVOUT,TEX-9249-TEX;n:type:ShaderForge.SFN_Panner,id:274,x:31835,y:32080,varname:node_274,prsc:2,spu:0.01,spv:-0.01|UVIN-6058-UVOUT,DIST-3930-OUT;n:type:ShaderForge.SFN_TexCoord,id:6058,x:31561,y:32150,varname:node_6058,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Tex2dAsset,id:9249,x:31592,y:32471,ptovrint:False,ptlb:CausticsShallow,ptin:_CausticsShallow,varname:node_9249,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ef6b3d0bf823c2641bdbacb033d8894f,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Panner,id:1421,x:31855,y:32241,varname:node_1421,prsc:2,spu:-0.04,spv:-0.01|UVIN-6058-UVOUT,DIST-3930-OUT;n:type:ShaderForge.SFN_Tex2d,id:3391,x:32261,y:32379,varname:node_3391,prsc:2,tex:ef6b3d0bf823c2641bdbacb033d8894f,ntxv:0,isnm:False|UVIN-1421-UVOUT,TEX-9249-TEX;n:type:ShaderForge.SFN_Lerp,id:4393,x:33395,y:32649,varname:node_4393,prsc:2|A-4931-OUT,B-7659-RGB,T-7239-OUT;n:type:ShaderForge.SFN_OneMinus,id:7239,x:33156,y:32801,varname:node_7239,prsc:2|IN-9865-OUT;n:type:ShaderForge.SFN_Add,id:6800,x:32397,y:32777,varname:node_6800,prsc:2|A-8824-UVOUT,B-6741-OUT;n:type:ShaderForge.SFN_Vector1,id:4252,x:31831,y:32891,varname:node_4252,prsc:2,v1:0.02;n:type:ShaderForge.SFN_Append,id:8015,x:32005,y:32842,varname:node_8015,prsc:2|A-8942-R,B-8942-G;n:type:ShaderForge.SFN_Multiply,id:6741,x:32197,y:32865,varname:node_6741,prsc:2|A-8015-OUT,B-4252-OUT;n:type:ShaderForge.SFN_Slider,id:8993,x:34119,y:32810,ptovrint:False,ptlb:metallic,ptin:_metallic,varname:node_8993,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_TexCoord,id:5269,x:31612,y:33487,varname:node_5269,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Panner,id:207,x:32066,y:33706,varname:node_207,prsc:2,spu:0.0121,spv:-0.012|UVIN-5269-UVOUT,DIST-3930-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:8696,x:32341,y:33790,ptovrint:False,ptlb:WaterNormal,ptin:_WaterNormal,varname:node_8696,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:ed1066aab3d29c947b2f28a0ebab2d4d,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Panner,id:174,x:32089,y:33907,varname:node_174,prsc:2,spu:-0.021,spv:0.012|UVIN-5269-UVOUT,DIST-3930-OUT;n:type:ShaderForge.SFN_Add,id:3986,x:33043,y:33751,varname:node_3986,prsc:2|A-2304-RGB,B-2031-RGB;n:type:ShaderForge.SFN_Divide,id:7641,x:33317,y:33694,varname:node_7641,prsc:2|A-3986-OUT,B-7897-OUT;n:type:ShaderForge.SFN_Vector1,id:7897,x:33053,y:33935,varname:node_7897,prsc:2,v1:2;n:type:ShaderForge.SFN_Tex2d,id:2031,x:32606,y:33974,varname:node_2031,prsc:2,tex:ed1066aab3d29c947b2f28a0ebab2d4d,ntxv:0,isnm:False|UVIN-174-UVOUT,TEX-8696-TEX;n:type:ShaderForge.SFN_Add,id:722,x:32516,y:32325,varname:node_722,prsc:2|A-9746-RGB,B-3391-RGB;n:type:ShaderForge.SFN_ComponentMask,id:8942,x:32865,y:32493,varname:node_8942,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-1316-OUT;n:type:ShaderForge.SFN_Slider,id:8912,x:33846,y:32901,ptovrint:False,ptlb:Glass,ptin:_Glass,varname:node_8912,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8886719,max:1;n:type:ShaderForge.SFN_Divide,id:1316,x:32677,y:32493,varname:node_1316,prsc:2|A-722-OUT,B-7046-OUT;n:type:ShaderForge.SFN_Vector1,id:7046,x:32397,y:32599,varname:node_7046,prsc:2,v1:2;n:type:ShaderForge.SFN_Multiply,id:4931,x:32751,y:32183,varname:node_4931,prsc:2|A-2684-OUT,B-722-OUT;n:type:ShaderForge.SFN_Slider,id:2684,x:32252,y:32082,ptovrint:False,ptlb:CausticsShallowValue,ptin:_CausticsShallowValue,varname:node_2684,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:5;n:type:ShaderForge.SFN_Slider,id:1559,x:31181,y:33698,ptovrint:False,ptlb:speed,ptin:_speed,varname:node_1559,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1,max:1;n:type:ShaderForge.SFN_Multiply,id:3930,x:31670,y:33739,varname:node_3930,prsc:2|A-1559-OUT,B-9772-TTR;n:type:ShaderForge.SFN_Time,id:9772,x:31273,y:33806,varname:node_9772,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7392,x:33020,y:33164,varname:node_7392,prsc:2|A-9136-OUT,B-7402-OUT;proporder:8181-7402-9249-8993-8696-8912-2684-1559;pass:END;sub:END;*/

Shader "Shader Forge/Water" {
    Properties {
        _Color ("Color", Color) = (0,0.2551723,1,1)
        _depth ("depth", Range(0, 1)) = 0.660608
        _CausticsShallow ("CausticsShallow", 2D) = "white" {}
        _metallic ("metallic", Range(0, 1)) = 0
        _WaterNormal ("WaterNormal", 2D) = "bump" {}
        _Glass ("Glass", Range(0, 1)) = 0.8886719
        _CausticsShallowValue ("CausticsShallowValue", Range(0, 5)) = 0.5
        _speed ("speed", Range(0, 1)) = 0.1
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
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
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _Color;
            uniform float _depth;
            uniform sampler2D _CausticsShallow; uniform float4 _CausticsShallow_ST;
            uniform float _metallic;
            uniform sampler2D _WaterNormal; uniform float4 _WaterNormal_ST;
            uniform float _Glass;
            uniform float _CausticsShallowValue;
            uniform float _speed;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 projPos : TEXCOORD7;
                UNITY_FOG_COORDS(8)
                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)
                    float4 ambientOrLightmapUV : TEXCOORD9;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                #ifdef LIGHTMAP_ON
                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;
                    o.ambientOrLightmapUV.zw = 0;
                #elif UNITY_SHOULD_SAMPLE_SH
                #endif
                #ifdef DYNAMICLIGHTMAP_ON
                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
                #endif
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_9772 = _Time;
                float node_3930 = (_speed*node_9772.a);
                float2 node_207 = (i.uv0+node_3930*float2(0.0121,-0.012));
                float3 node_2304 = UnpackNormal(tex2D(_WaterNormal,TRANSFORM_TEX(node_207, _WaterNormal)));
                float2 node_174 = (i.uv0+node_3930*float2(-0.021,0.012));
                float3 node_2031 = UnpackNormal(tex2D(_WaterNormal,TRANSFORM_TEX(node_174, _WaterNormal)));
                float3 normalLocal = ((node_2304.rgb+node_2031.rgb)/2.0);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Glass;
                float perceptualRoughness = 1.0 - _Glass;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
                    d.ambient = 0;
                    d.lightmapUV = i.ambientOrLightmapUV;
                #else
                    d.ambient = i.ambientOrLightmapUV;
                #endif
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _metallic;
                float specularMonochrome;
                float2 node_274 = (i.uv0+node_3930*float2(0.01,-0.01));
                float4 node_9746 = tex2D(_CausticsShallow,TRANSFORM_TEX(node_274, _CausticsShallow));
                float2 node_1421 = (i.uv0+node_3930*float2(-0.04,-0.01));
                float4 node_3391 = tex2D(_CausticsShallow,TRANSFORM_TEX(node_1421, _CausticsShallow));
                float3 node_722 = (node_9746.rgb+node_3391.rgb);
                float2 node_8942 = (node_722/2.0).rg;
                float node_9865 = saturate(((max(0, LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sceneUVs)) - _ProjectionParams.g)-partZ)*_depth));
                float3 diffuseColor = lerp(lerp((_CausticsShallowValue*node_722),tex2D( _GrabTexture, (sceneUVs.rg+(float2(node_8942.r,node_8942.g)*0.02))).rgb,(1.0 - node_9865)),_Color.rgb,node_9865); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                half surfaceReduction;
                #ifdef UNITY_COLORSPACE_GAMMA
                    surfaceReduction = 1.0-0.28*roughness*perceptualRoughness;
                #else
                    surfaceReduction = 1.0/(roughness*roughness + 1.0);
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                indirectSpecular *= surfaceReduction;
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += gi.indirect.diffuse;
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor,node_9865);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _Color;
            uniform float _depth;
            uniform sampler2D _CausticsShallow; uniform float4 _CausticsShallow_ST;
            uniform float _metallic;
            uniform sampler2D _WaterNormal; uniform float4 _WaterNormal_ST;
            uniform float _Glass;
            uniform float _CausticsShallowValue;
            uniform float _speed;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float3 normalDir : TEXCOORD4;
                float3 tangentDir : TEXCOORD5;
                float3 bitangentDir : TEXCOORD6;
                float4 projPos : TEXCOORD7;
                LIGHTING_COORDS(8,9)
                UNITY_FOG_COORDS(10)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 node_9772 = _Time;
                float node_3930 = (_speed*node_9772.a);
                float2 node_207 = (i.uv0+node_3930*float2(0.0121,-0.012));
                float3 node_2304 = UnpackNormal(tex2D(_WaterNormal,TRANSFORM_TEX(node_207, _WaterNormal)));
                float2 node_174 = (i.uv0+node_3930*float2(-0.021,0.012));
                float3 node_2031 = UnpackNormal(tex2D(_WaterNormal,TRANSFORM_TEX(node_174, _WaterNormal)));
                float3 normalLocal = ((node_2304.rgb+node_2031.rgb)/2.0);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float gloss = _Glass;
                float perceptualRoughness = 1.0 - _Glass;
                float roughness = perceptualRoughness * perceptualRoughness;
                float specPow = exp2( gloss * 10.0 + 1.0 );
////// Specular:
                float NdotL = saturate(dot( normalDirection, lightDirection ));
                float LdotH = saturate(dot(lightDirection, halfDirection));
                float3 specularColor = _metallic;
                float specularMonochrome;
                float2 node_274 = (i.uv0+node_3930*float2(0.01,-0.01));
                float4 node_9746 = tex2D(_CausticsShallow,TRANSFORM_TEX(node_274, _CausticsShallow));
                float2 node_1421 = (i.uv0+node_3930*float2(-0.04,-0.01));
                float4 node_3391 = tex2D(_CausticsShallow,TRANSFORM_TEX(node_1421, _CausticsShallow));
                float3 node_722 = (node_9746.rgb+node_3391.rgb);
                float2 node_8942 = (node_722/2.0).rg;
                float node_9865 = saturate(((max(0, LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sceneUVs)) - _ProjectionParams.g)-partZ)*_depth));
                float3 diffuseColor = lerp(lerp((_CausticsShallowValue*node_722),tex2D( _GrabTexture, (sceneUVs.rg+(float2(node_8942.r,node_8942.g)*0.02))).rgb,(1.0 - node_9865)),_Color.rgb,node_9865); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = abs(dot( normalDirection, viewDirection ));
                float NdotH = saturate(dot( normalDirection, halfDirection ));
                float VdotH = saturate(dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, roughness );
                float normTerm = GGXTerm(NdotH, roughness);
                float specularPBL = (visTerm*normTerm) * UNITY_PI;
                #ifdef UNITY_COLORSPACE_GAMMA
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                #endif
                specularPBL = max(0, specularPBL * NdotL);
                #if defined(_SPECULARHIGHLIGHTS_OFF)
                    specularPBL = 0.0;
                #endif
                specularPBL *= any(specularColor) ? 1.0 : 0.0;
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                fixed4 finalRGBA = fixed4(finalColor * node_9865,0);
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
            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityMetaPass.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON
            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE
            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _GrabTexture;
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _Color;
            uniform float _depth;
            uniform sampler2D _CausticsShallow; uniform float4 _CausticsShallow_ST;
            uniform float _metallic;
            uniform float _Glass;
            uniform float _CausticsShallowValue;
            uniform float _speed;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                float2 texcoord2 : TEXCOORD2;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float4 posWorld : TEXCOORD3;
                float4 projPos : TEXCOORD4;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.uv2 = v.texcoord2;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i) : SV_Target {
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
                float2 sceneUVs = (i.projPos.xy / i.projPos.w);
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
                UnityMetaInput o;
                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );
                
                o.Emission = 0;
                
                float4 node_9772 = _Time;
                float node_3930 = (_speed*node_9772.a);
                float2 node_274 = (i.uv0+node_3930*float2(0.01,-0.01));
                float4 node_9746 = tex2D(_CausticsShallow,TRANSFORM_TEX(node_274, _CausticsShallow));
                float2 node_1421 = (i.uv0+node_3930*float2(-0.04,-0.01));
                float4 node_3391 = tex2D(_CausticsShallow,TRANSFORM_TEX(node_1421, _CausticsShallow));
                float3 node_722 = (node_9746.rgb+node_3391.rgb);
                float2 node_8942 = (node_722/2.0).rg;
                float node_9865 = saturate(((max(0, LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sceneUVs)) - _ProjectionParams.g)-partZ)*_depth));
                float3 diffColor = lerp(lerp((_CausticsShallowValue*node_722),tex2D( _GrabTexture, (sceneUVs.rg+(float2(node_8942.r,node_8942.g)*0.02))).rgb,(1.0 - node_9865)),_Color.rgb,node_9865);
                float specularMonochrome;
                float3 specColor;
                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _metallic, specColor, specularMonochrome );
                float roughness = 1.0 - _Glass;
                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;
                
                return UnityMetaFragment( o );
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
