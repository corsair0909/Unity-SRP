Shader "Custom/UnlitShader"
{
    Properties
    {
        _MainTex ("MainTex",2D) = "White"{}
        _BaseColor("Color",Color) = (1.0,1.0,1.0,1.0)
        _CutOff ("CutOff",range(0,1)) = 0
        [Toggle(_CLIPPING)] _Clipping("Alpha Clip",float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend",float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DstBlend",float) = 1
        [Enum(Off,0,On,1)] _ZWrite ("ZWrite",float) = 1
    }

    SubShader
    {
            HLSLINCLUDE
            //#include "Assets/Shader/UnlitPass.hlsl"
            #include "Assets/Shader/UnlitPassCommon.hlsl"
            #pragma multi_compile_instancing //GPU Instancing

            //SRP Batcher
            // CBUFFER_START(UnityPerMaterial)
            // float4 _BaseColor;
            // CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            UNITY_INSTANCING_BUFFER_START(UnityPerMaterial)
                UNITY_DEFINE_INSTANCED_PROP(float4,_MainTex_ST)
                UNITY_DEFINE_INSTANCED_PROP(float4,_BaseColor)
                UNITY_DEFINE_INSTANCED_PROP(float,_CutOff)
            UNITY_INSTANCING_BUFFER_END(UnityPerMaterial)
            
            ENDHLSL
        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]
            
            HLSLPROGRAM
            #pragma shader_feature _CLIPPING
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment

            //float4 _BaseColor;
            struct Attributes
            {
                float3 positionOS : POSITION;
                float2 baseUV     : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID // GPU Instance 宏
            };

            struct Varing
            {
                float4 positionOS : SV_POSITION;
                float2 UV         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            Varing UnlitPassVertex(Attributes input)
            {
                Varing o;
                UNITY_SETUP_INSTANCE_ID(input); // GPU Instance宏
                UNITY_TRANSFER_INSTANCE_ID(input,o); // 输出位置和索引
                //方法来自SpaceTransforms.hlsl
                float3 positionWS = TransformObjectToWorld(input.positionOS);
                o.positionOS = TransformWorldToHClip(positionWS);
                float4 baseUVST = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_MainTex_ST);
                o.UV = input.baseUV * baseUVST.xy+baseUVST.zw;
                return o;
            }

            float4 UnlitPassFragment(Varing input) : SV_TARGET
            {
                UNITY_SETUP_INSTANCE_ID(input);
                float4 var_MainTex = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,input.UV);
                float4 baseColor = UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_BaseColor);
                float4 finalCol = baseColor*var_MainTex;
                #ifdef  _CLIPPING
                    clip(finalCol.a - UNITY_ACCESS_INSTANCED_PROP(UnityPerMaterial,_CutOff));
                #endif
                
                return finalCol;
                //return _BaseColor;
            }
            ENDHLSL
        }
    }
}
