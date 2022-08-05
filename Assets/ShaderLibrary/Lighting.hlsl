#ifndef CUSTOM_LIGHTING_INCLUDE
#define CUSTOM_LIGHTING_INCLUDE

#include "Assets/ShaderLibrary/Surface.hlsl"
#include "Assets/ShaderLibrary/CustomLight.hlsl"

float3 GetIncomingLight(Surface surface,CLight light)
{
    return saturate(dot(surface.normal,light.Dircetion)) * light.Color;
}
float3 GetLighting(Surface surface)
{
   float3 LightColor = 0;
   for (int i = 0; i < GetDirectionLightCount(); ++i)
   {
       LightColor += GetIncomingLight(surface,GetDirectionalLight(i));
   }
    return LightColor;
}

#endif
