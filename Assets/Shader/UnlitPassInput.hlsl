#ifndef CUSTOM_UNLIT_PASS_INPUT_INCLUDE
#define CUSTOM_UNLIT_PASS_INPUT_INCLUDE
#endif

CBUFFER_START(UnityPerDraw)
    float4x4 unity_ObjectToWorld;
    float4x4 unity_WorldToObject;
    float4 LODFade;
    //包含不再需要的转换信息
    real4 unity_WorldTransformParams;
    float3 _WorldSpaceCameraPos;
CBUFFER_END

float4x4 unity_MatrixVP; // 视图投影矩阵，它使unity_ObjectToWorld矩阵可用
float4x4 unity_MatrixV;
float4x4 glstate_matrix_projection;//P矩阵