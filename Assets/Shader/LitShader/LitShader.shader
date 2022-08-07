Shader "Custom/LitShader"
{
    Properties
    {
        _MainTex ("MainTex",2D) = "White"{}
        _BaseColor("Color",Color) = (1.0,1.0,1.0,1.0)
        _CutOff ("CutOff",range(0,1)) = 0
        _Metallic ("Metallic",Range(0,1)) = 0
        _Smoothness ("Smoothness",Range(0,1)) = 0.5
        [Toggle(_CLIPPING)] _Clipping("Alpha Clip",float) = 0
        [Toggle(_PREMULTIPLY_ALPHA)] _PermulAlpha("Premultiply Alpha",float) = 0
        
        [Space(10)]
        [Header(BlendMode)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend",float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DstBlend",float) = 1
        [Enum(Off,0,On,1)] _ZWrite ("ZWrite",float) = 1
    }

    SubShader
    {
            HLSLINCLUDE
            //#include "Assets/Shader/LitPass.hlsl"
            #include "Assets/Shader/UnlitPassCommon.hlsl"
            #include "Assets/ShaderLibrary/Lighting.hlsl"

            #pragma multi_compile_instancing //GPU Instancing

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
                UNITY_DEFINE_INSTANCED_PROP(float4,_MainTex_ST)
                UNITY_DEFINE_INSTANCED_PROP(float4,_BaseColor)
                UNITY_DEFINE_INSTANCED_PROP(float,_CutOff)
                UNITY_DEFINE_INSTANCED_PROP(float,_Metallic)
                UNITY_DEFINE_INSTANCED_PROP(float,_Smoothness)
            UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)
            
            ENDHLSL
        Pass
        {
            Tags{"LightMode" = "CustomLit"}
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            
            HLSLPROGRAM
            #pragma shader_feature _ISCLIPPING
            #pragma shader_feature _PREMULTIPLY_ALPHA
            #pragma vertex LitPassVertex
            #pragma fragment LitPassFragment
            #pragma target 3.5


            //float4 _BaseColor;
            struct Attributes
            {
                float3 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 baseUV       : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // GPU Instance 宏
            };
            
            struct Varing
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float2 UV         : TEXCOORD1;
                float3 NormalWS   : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            Varing LitPassVertex(Attributes input)
            {
                Varing o;
                UNITY_SETUP_INSTANCE_ID(input); // GPU Instance宏
                UNITY_TRANSFER_INSTANCE_ID(input,o); // 输出位置和索引
                o.positionCS = TransformObjectToHClip(input.positionOS);
                o.positionWS = TransformObjectToWorld(input.positionOS);
                float4 BaseST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_MainTex_ST);
                o.UV = input.baseUV * BaseST.xy + BaseST.zw;
                o.NormalWS = TransformObjectToWorldNormal(input.normalOS,true);
                return o;
            }
            
            float4 LitPassFragment(Varing input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.UV);
                float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_BaseColor);

                #ifdef _ISCLIPPING
                    clip(baseColor.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_CutOff));
                #endif
                Surface surface;
                
                surface.normal = normalize(input.NormalWS);//标准化来平滑法线，减少失真
                surface.color =  baseColor.rgb;
                surface.alpha = baseColor.a;
                surface.metallic = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_Metallic);
                surface.smoothness = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_Smoothness);
                surface.viewDirection = normalize(_WorldSpaceCameraPos-input.positionWS);
                #ifdef _PREMULTIPLY_ALPHA
                    BRDF brdf = GetBRDF(surface,true);
                #else
                    BRDF brdf = GetBRDF(surface);
                #endif
                
                float3 Lightcolor = GetLighting(surface,brdf);
                

                float3 finalCol = surface.color * Lightcolor;
                return float4(finalCol,surface.alpha);
            }

            ENDHLSL
        }
    }
    CustomEditor "CustomShaderGUI"
}
