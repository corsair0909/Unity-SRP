using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRender
{
    private ScriptableRenderContext _context;
    private Camera _camera;
    private const string BufferName = "Render Camera";
    private CommandBuffer _buffer = new CommandBuffer() { name = BufferName };
    private CullingResults _results;
    private ShaderTagId _shaderID = new ShaderTagId("SRPDefaultUnlit");
    public void Render(ScriptableRenderContext context,Camera camera)
    {
        this._camera = camera;
        this._context = context;
        
        PerpareCamera();
        DrawUGUI();
        if (!Cull())
        {
            return;
        }
        
        SetUp();
        DrawunSupportGemoetry();
        DrawVisibleGeometry();
        DrawGizmos();
        Submit();
    }
    void DrawVisibleGeometry()
    {
        //绘制不透明物体
        var sortSetting = new SortingSettings(){criteria = SortingCriteria.CommonOpaque};
        var drawSetting = new DrawingSettings(_shaderID,sortSetting);
        var filterSetting = new FilteringSettings(RenderQueueRange.opaque);
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
    
    bool Cull()
    {
        //获取剔除参数
        if (_camera.TryGetCullingParameters(out ScriptableCullingParameters parameters))
        {
            _results = _context.Cull(ref parameters);
            return true;
        }
        else
        {
            return false;
        }
    }
}
