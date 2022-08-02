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
            #pragma vertex UnlitPassVertex
            #pragma fragment UnlitPassFragment
            ENDHLSL
        }
    }
}
