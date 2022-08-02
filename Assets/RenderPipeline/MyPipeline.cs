using UnityEngine;
using UnityEngine.Rendering;

public class MyPipeline : RenderPipeline//扩展抽象类，可以实现IRenderPipeline
{
    protected override void Render(ScriptableRenderContext context, Camera[] cameras)
    {
        throw new System.NotImplementedException();
    }
}
