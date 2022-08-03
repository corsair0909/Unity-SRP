Shader "Unlit/CustomRPShader"
{
    Properties
    {
        _BaseColor("Color",Color) = (1.0,1.0,1.0,1.0)
    }

    SubShader
    {
            HLSLINCLUDE
            //#include "Assets/Shader/UnlitPass.hlsl"
            #include "Assets/Shader/UnlitPassCommon.hlsl"
            #pragma multi_compile_instancing //GPU Instancing

            //SRP Batcher
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
            CBUFFER_END
            
            ENDHLSL
        Pass
        {
            HLSLPROGRAM

            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            
            float4 UnlitPassVertex(float4 positionOS : POSITION) : SV_POSITION
            {
                //方法来自SpaceTransforms.hlsl
                float3 positionWS = TransformObjectToWorld(positionOS);
                return TransformWorldToHClip(positionWS);
            }

            float4 UnlitPassFragment() : SV_TARGET
            {
                return _BaseColor;
            }
            ENDHLSL
        }
    }
}
