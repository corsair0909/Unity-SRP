#ifndef CUSTOM_UNLIT_PASS_INCLUDE
#define CUSTOM_UNLIT_PASS_INCLUDE
#endif

#include "Assets/Shader/UnlitPassCommon.hlsl"

float4 _BaseColor;

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