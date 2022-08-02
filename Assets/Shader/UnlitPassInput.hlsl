#ifndef CUSTOM_UNLIT_PASS_INCLUDE
#define CUSTOM_UNLIT_PASS_INCLUDE
#endif

float4 UnlitPassVertex(float4 positionOS : POSITION) : SV_POSITION
{
    return float4(positionOS.xyz,1);
}
float4 UnlitPassFragment() : SV_Target
{
    return 0.0;
}