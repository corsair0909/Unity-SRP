#ifndef CUSTOM_LIGHT_INCLUDE
#define CUSTOM_LIGHT_INCLUDE

#define MAX_DIRECTIONAL_LIGHT_COUNT 4
CBUFFER_START(CustomLight)
float _DirectionalLightCount;
float3 _DirectionalLightColors[MAX_DIRECTIONAL_LIGHT_COUNT];
float3 _DirectionalLightDirections[MAX_DIRECTIONAL_LIGHT_COUNT];

CBUFFER_END

struct CustomLight
{
    float3 Color;
    float3 Dircetion;
};

int GetDirectionLightCount()
{
    return _DirectionalLightCount;
}

CustomLight GetDirectionalLight(int index)
{
    CustomLight light;
    light.Color = _DirectionalLightColors[index];
    light.Dircetion = _DirectionalLightDirections[index];
    return light;
}

#endif
