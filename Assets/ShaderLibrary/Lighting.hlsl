#ifndef CUSTOM_LIGHTING_INCLUDE
#define CUSTOM_LIGHTING_INCLUDE

#include "Assets/ShaderLibrary/Surface.hlsl"
#include "Assets/ShaderLibrary/CustomLight.hlsl"
#include "Assets/ShaderLibrary/BRDF.hlsl"


float3 GetIncomingLight(Surface surface,BRDF brdf,CLight light)
{
    float3 IncomingLight = saturate(dot(surface.normal,light.Dircetion)) * light.Color;
    return DirecBRDF(surface,brdf,light) * IncomingLight;
}
float3 GetLighting(Surface surface,BRDF brdf)
{
   float3 LightColor = 0;
   for (int i = 0; i < GetDirectionLightCount(); ++i)
   {
       LightColor += GetIncomingLight(surface,brdf,GetDirectionalLight(i));
   }
    return LightColor;
}



#endif
