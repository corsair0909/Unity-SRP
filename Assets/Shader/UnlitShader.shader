Shader "Unlit/CustomRPShader"
{
    Properties
    {
        
    }
    SubShader
    {
       
        Pass
        {
            HLSLPROGRAM
            #include "Assets/Shader/UnlitPass.hlsl"
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            ENDHLSL
        }
    }
}
