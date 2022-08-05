using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 获得负责渲染的管线的实例对象
/// </summary>
[CreateAssetMenu(menuName = "Rendering/MyPipeline",fileName = "MyPipelineAsset")]
public class MyRenderPipeline : RenderPipelineAsset//管线资产父类
{

    [SerializeField]
    private bool useDynamicBatching,useGPUInstancBatching,useSRPBatching;
    //返回管线的实例对象
    protected override RenderPipeline CreatePipeline()
    {
        return new MyPipeline(useDynamicBatching,useGPUInstancBatching,useSRPBatching);
    }
}
