using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

public class MyPipeline : RenderPipeline//扩展抽象类，可以实现IRenderPipeline
{
    private CameraRender _render = new CameraRender();//渲染类
    
    public MyPipeline()
    {
        GraphicsSettings.useScriptableRenderPipelineBatching = true;
    }
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (Camera cam in cameras)
        {
            _render.Render(context,cam);
        }
    }


}
