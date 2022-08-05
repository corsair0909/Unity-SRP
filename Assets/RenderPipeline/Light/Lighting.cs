using UnityEngine;
using UnityEngine.Rendering;

public class Light
{
    private const string _bufferName = "Lighting";
    private CommandBuffer _buffer = new CommandBuffer(){name = _bufferName};

    private static int
        DirectionLightColor = Shader.PropertyToID("_DirectionalLightColor"),
        DirectionLightDir = Shader.PropertyToID("_DirectionalLightDirection");

    public void SetUp(ScriptableRenderContext context)
    {
        _buffer.BeginSample(_bufferName);
        SetDirectionalLight();
        _buffer.EndSample(_bufferName);
        context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }

    /// <summary>
    /// CommandBuffer.SetGlobalVector将灯光数据发送到GPU。颜色是灯光在线性空间中的颜色，而方向是灯光变换的正向向量取反。
    /// </summary>
    void SetDirectionalLight()
    {
        _buffer.SetGlobalVector(DirectionLightColor,RenderSettings.sun.color.linear * RenderSettings.sun.intensity);
        _buffer.SetGlobalVector(DirectionLightDir,(RenderSettings.sun.transform.forward * -1));
    }
}
