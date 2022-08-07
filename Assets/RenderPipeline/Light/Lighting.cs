using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class Lighting
{
    private const string _bufferName = "Lighting";
    private CommandBuffer _buffer = new CommandBuffer(){name = _bufferName};
    private CullingResults _results;

    private const int maxDirLightCount = 4;

    private Shadows _shadows = new Shadows();

    private static int
        //添加多光源支持，
        //Unity为着色器的每个属性名称都会分配一个整个游戏中唯一的整数类型ID
        //比传递字符串更有效率，但该数字在不同机器中不同，不要通过存储或者网络发送该数字。
        DirLightCountID = Shader.PropertyToID("_DirectionalLightCount"),
        DirLightColorID = Shader.PropertyToID("_DirectionalLightColors"),
        DirLightDirectionalID = Shader.PropertyToID("_DirectionalLightDirections");

    private static Vector4[]
        DirLightColors = new Vector4[maxDirLightCount],
        DirLightDirectionals = new Vector4[maxDirLightCount];

    public void SetUp(ScriptableRenderContext context,CullingResults results,ShadowSetting shadowSetting)
    {
        this._results = results;
        _buffer.BeginSample(_bufferName);
        _shadows.Setup(context,results,shadowSetting);
        SetVisibiletyLight();
        _shadows.Render();
        _buffer.EndSample(_bufferName);
        context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }

    void SetVisibiletyLight()
    {
        //剔除结果的visibleLights属性检索所需的数据。它是VisibleLight元素类型的Unity.Collections.NativeArray形式提供。
        NativeArray<VisibleLight> visibleLights = _results.visibleLights;
        int dirLightCount=0;
        //处理多定向光源
        for (int i = 0; i < visibleLights.Length; i++)
        {
            VisibleLight _visibleLight = visibleLights[i];
            if (_visibleLight.lightType == LightType.Directional)
            {
                SetDirectionalLight(dirLightCount++,ref _visibleLight);
                if (dirLightCount>maxDirLightCount)
                {
                    break;
                }
            }
        }
        _buffer.SetGlobalFloat(DirLightCountID,visibleLights.Length);
        _buffer.SetGlobalVectorArray(DirLightColorID,DirLightColors);
        _buffer.SetGlobalVectorArray(DirLightDirectionalID,DirLightDirectionals);
    }

    //CommandBuffer.SetGlobalVector将灯光数据发送到GPU。颜色是灯光在线性空间中的颜色，而方向是灯光变换的正向向量取反。
    // void SetDirectionalLight()
    // {
    //      _buffer.SetGlobalVector(DirLightCountID,RenderSettings.sun.color.linear * RenderSettings.sun.intensity);
    //      _buffer.SetGlobalVector(DirLightColorID,(RenderSettings.sun.transform.forward * -1));
    // }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <param name="visibleLight"></param>结构体十分大，当作普通参数传递进去会复制一份结构体，，使用ref传引用
    void SetDirectionalLight(int index, ref VisibleLight visibleLight)
    {
        DirLightColors[index] = visibleLight.finalColor;
        //visibleLight.localToWorldMatrix.GetColumn(2) == transform.forward. 前向矢量是矩阵第三列
        DirLightDirectionals[index] =-visibleLight.localToWorldMatrix.GetColumn(2);
        _shadows.ReserveDirectionalShadows(visibleLight.light,index);
    }

    public void Cleanup()
    {
        _shadows.Cleanup();
    }
}
