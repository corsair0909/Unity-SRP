#ifndef CUSTOM_UNLIT_PASS_COMMON_INCLUDE
#define CUSTOM_UNLIT_PASS_COMMON_INCLUDE
#endif

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Assets/Shader/UnlitPassInput.hlsl"
//SpaceTransforms.hlsl代码中没有unity_ObjectToWorld
//先执行此操作。之后，所有UNITY_MATRIX_M出现都将被unity_ObjectToWorld取代
#define UNITY_MATRIX_M unity_ObjectToWorld
#define UNITY_MATRIX_I_M unity_WorldToObject
#define UNITY_MATRIX_V unity_MatrixV
#define UNITY_MATRIX_VP unity_MatrixVP
#define UNITY_MATRIX_P glstate_matrix_projection

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"


