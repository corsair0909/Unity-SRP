Shader "Unlit/CustomRPShader"
{
    Properties
    {
        _BaseColor("Color",Color) = (1.0,1.0,1.0,1.0)
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
