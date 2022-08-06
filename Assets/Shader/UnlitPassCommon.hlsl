#ifndef CUSTOM_UNLIT_PASS_COMMON_INCLUDE
#define CUSTOM_UNLIT_PASS_COMMON_INCLUDE
#endif

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"

#include "Assets/Shader/UnlitPassInput.hlsl"
//SpaceTransforms.hlsl代码中没有unity_ObjectToWorld
//先执行此操作。之后，所有UNITY_MATRIX_M出现都将被unity_ObjectToWorld取代
#define UNITY_MATRIX_M unity_ObjectToWorld
#define UNITY_MATRIX_I_M unity_WorldToObject
#define UNITY_MATRIX_V unity_MatrixV
#define UNITY_MATRIX_VP unity_MatrixVP
#define UNITY_MATRIX_P glstate_matrix_projection

//重新定义宏来访问实例数据数组，进行这项工作需要知道当前正在渲染的对象的索引，
//索引是通过顶点数据提供的，UnityInstancing.hlsl定义了宏来简化此过程
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"


