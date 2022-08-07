using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRender
{
    private ScriptableRenderContext _context;
    private Camera _camera;
    private const string BufferName = "Render Camera";
    private CommandBuffer _buffer = new CommandBuffer() { name = BufferName };
    private CullingResults _results;
    private Lighting _lighting = new Lighting();
    static ShaderTagId //构造函数中传递的字符对应 "LightMode"类型，
        _unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit"),
        _ligShaderTagId = new ShaderTagId("CustomLit");
    public void Render(ScriptableRenderContext context,Camera camera,bool useDynamicBatching,bool useGPUInstancBatching,
        ShadowSetting shadowSettings)
    {
        this._camera = camera;
        this._context = context;
        
        PerpareCamera();
        DrawUGUI();
        if (!Cull(shadowSettings.maxDistance))
        {
            return;
        }
        _buffer.BeginSample(BufferName);
        ExecuteCommanderBuffer();
        _lighting.SetUp(context,_results,shadowSettings);
        _buffer.EndSample(BufferName);
        SetUp();
        DrawunSupportGemoetry();
        DrawVisibleGeometry(useDynamicBatching,useGPUInstancBatching);
        DrawGizmos();
        _lighting.Cleanup();
        Submit();
    }
    void DrawVisibleGeometry(bool useDynamicBatching,bool useGPUInstancBatching)
    {
        //绘制不透明物体
        
        //排序参数设定
        var sortSetting = new SortingSettings(){criteria = SortingCriteria.CommonOpaque};
        //绘制参数设定
        var drawSetting = new DrawingSettings(_unlitShaderTagId,sortSetting)
            {enableDynamicBatching = useDynamicBatching,enableInstancing = useGPUInstancBatching};
        drawSetting.SetShaderPassName(1,_ligShaderTagId);
        //过滤参数设定,传入整数标识队列
        var filterSetting = new FilteringSettings(RenderQueueRange.opaque);
        //绘制方法 cull参数、绘制参数、过滤参数
        _context.DrawRenderers(_results,ref drawSetting,ref filterSetting);
        
        //绘制天空盒
        _context.DrawSkybox(_camera);
        
        //最后绘制透明物体
        sortSetting.criteria = SortingCriteria.CommonTransparent;
        drawSetting.sortingSettings = sortSetting;
        filterSetting.renderQueueRange = RenderQueueRange.transparent;
        _context.DrawRenderers(_results,ref drawSetting,ref filterSetting);
    }


    
    void SetUp()
    {
        _context.SetupCameraProperties(_camera);//设置相机视图投影矩阵
        
        //TODO 没明白啥意思
        CameraClearFlags flags = _camera.clearFlags;
        _buffer.ClearRenderTarget(flags<=CameraClearFlags.Depth,
                                    flags==CameraClearFlags.Color,
                                    flags == CameraClearFlags.Color?
                                    _camera.backgroundColor.linear:Color.clear); //清除渲染目标
        
        _buffer.BeginSample(SampleName); //commanderBuffer开始
        ExecuteCommanderBuffer();

    }    
    
    void Submit()
    {
        _buffer.EndSample(SampleName);//commanderBuffer结束
        ExecuteCommanderBuffer();
        _context.Submit();
    }

    void ExecuteCommanderBuffer()
    {
        _context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }
    
    bool Cull(float maxShadowDistance)
    {
        //获取剔除参数
        if (_camera.TryGetCullingParameters(out ScriptableCullingParameters parameters))
        {
            //设置最大阴影距离，相机看不到的阴影无需渲染，在远平面和阴影距离之间选择小的距离
            parameters.shadowDistance = Mathf.Min(maxShadowDistance,_camera.farClipPlane);
            _results = _context.Cull(ref parameters);
            return true;
        }
        else
        {
            return false;
        }
    }
}
