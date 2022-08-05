using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

public class MyPipeline : RenderPipeline//扩展抽象类，可以实现IRenderPipeline
{
    private bool useDynamicBatching,useGPUInstancBatching;
    private CameraRender _render = new CameraRender();//渲染类
    
    public MyPipeline(bool useDynamicBatching,bool useGPUInstancBatching,bool useSRPBatching)
    {
        this.useDynamicBatching = useDynamicBatching;
        this.useGPUInstancBatching = useGPUInstancBatching;
        //在调用产生实例时开启SRP Batcher
        GraphicsSettings.useScriptableRenderPipelineBatching = useSRPBatching;
        //Unity默认不会将灯光转换到线性空间下，需要手动开启
        GraphicsSettings.lightsUseLinearIntensity = true;
    }
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (Camera cam in cameras)
        {
            _render.Render(context,cam,useDynamicBatching,useGPUInstancBatching);
        }
    }


}
