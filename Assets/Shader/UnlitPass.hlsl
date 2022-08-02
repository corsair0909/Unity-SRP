#ifndef CUSTOM_UNLIT_PASS_INCLUDE
#define CUSTOM_UNLIT_PASS_INCLUDE
#endif

#include "Assets/Shader/UnlitPassCommon.hlsl"

float4 UnlitPassVertex(float4 positionOS : POSITION) : SV_POSITION
{
    float3 positionWS = TransformObjToWorld(positionOS.xyz);
    return TransformWorldToHClip(positionWS);
}
float4 UnlitPassFragment() : SV_Target
{
    return 0.0;
}