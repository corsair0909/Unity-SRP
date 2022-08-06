#ifndef CUSTOM_BRDF_INCLUDE
#define CUSTOM_BRDF_INCLUDE

//#include "Assets/ShaderLibrary/Surface.hlsl"

#define MIN_REFLECTIVITY 0.04

struct BRDF
{
    float3 diffuse;
    float3 specular;
    float roughness;
};

float OneMinusReflectivity (float metallic)
{
    //一些光还会从电介质表面返回来，使其具有亮点，
    //URP的反射范围在 0-0.96,调整范围
    float range = 1.0 - MIN_REFLECTIVITY;
    return range - metallic * range;
}
float Square (float v)
{
    return v * v;
}

BRDF GetBRDF( Surface surface,bool applyAlphaToDiffuse = false)
{
    BRDF brdf;
    //不同表面反射方式不同，通常金属会镜面反射所有光，漫反射为0
    float oneMinusReflectivity = OneMinusReflectivity(surface.metallic);
    brdf.diffuse = surface.color * oneMinusReflectivity;
    if (applyAlphaToDiffuse)
    {
        brdf.diffuse *= surface.alpha;
    }
    //以一种方式反射的光，不能全部以另一种方式反射
    //但这样忽略了金属会影响镜面反射颜色的事实
    //brdf.specular = surface.color - brdf.diffuse;
    brdf.specular = lerp(MIN_REFLECTIVITY,surface.color,surface.metallic);
    //粗糙度与平滑度相反
    //将粗糙度平方使得与迪士尼照明模型匹配
    float perceputalRoughness = PerceptualSmoothnessToPerceptualRoughness(surface.smoothness);
    brdf.roughness = PerceptualRoughnessToRoughness(perceputalRoughness);
    return brdf;
}

//Minimalist CookTorranve BRDF 的变体，
//r：粗糙度 N：表面法线 H：半角向量
// n = 4r + 2
float SpecularStrength(Surface surface,BRDF brdf,CLight light)
{
    float3 halfVec = SafeNormalize(light.Dircetion+surface.viewDirection);
    float NH2 = Square(saturate(dot(surface.normal,halfVec)));
    float LH2 = Square(saturate(dot(light.Dircetion,halfVec)));
    float r2 = Square(brdf.roughness);
    float d2 = Square(NH2*(r2 - 1)+1.00001);
    float n = 4 * brdf.roughness + 2;
    return r2/(d2 * max(0.1,LH2)) * n;
}

float3 DirecBRDF (Surface surface,BRDF brdf,CLight light)
{
    return (SpecularStrength(surface,brdf,light) * brdf.specular) + brdf.diffuse;
}


#endif
