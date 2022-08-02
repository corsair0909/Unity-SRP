Shader "Unlit/CustomRPShader"
{
    Properties
    {
        //_BaseColor("Color",Color) = (1.0,1.0,1.0,1.0)
    }
    SubShader
    {
       
        Pass
        {
            HLSLPROGRAM
            #include "Assets/Shader/UnlitPass.hlsl"
            #pragma multi_compile_instancing //GPU Instancing
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            ENDHLSL
        }
    }
}
