using UnityEditor;
using UnityEngine.Profiling;

using UnityEngine;
using UnityEngine.Rendering;

partial class CameraRender
{
    private ScriptableRenderContext _context;
    private Camera _camera;
    private const string BufferName = "Render Camera";
    private CommandBuffer _buffer = new CommandBuffer() { name = BufferName };
    private CullingResults _results;
    private ShaderTagId _shaderID = new ShaderTagId("SRPDefaultUnlit");

    partial void DrawunSupportGemoetry();
    partial void DrawGizmos();
    partial void DrawUGUI();
    partial void PerpareCamera();
    #if UNITY_EDITOR
    private static Material _errorMaterial;
    private static ShaderTagId[] legacyShaderTagIds =
            {new ShaderTagId("Always"), 
            new ShaderTagId("ForwardBase"), 
            new ShaderTagId("PrepassBase"), 
            new ShaderTagId("Vertex"),
            new ShaderTagId("VertexLMRGBM"),
            new ShaderTagId("VertexLM")};
    
    /// <summary>
    /// 不支持材质绘制
    /// </summary>
    partial void DrawunSupportGemoetry()
    {
        if (!_errorMaterial)
        {
            _errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));
        }
        //overrideMaterial 设置此组所有的渲染器shader
        var drawSetting = new DrawingSettings(legacyShaderTagIds[0], 
                new SortingSettings(_camera){criteria = SortingCriteria.CommonOpaque})
            {overrideMaterial = _errorMaterial};
        //defaultValue：不进行筛选的默认值
        var filterSetting = FilteringSettings.defaultValue;
        for (int i = 1; i < legacyShaderTagIds.Length; i++)
        {
            drawSetting.SetShaderPassName(i,legacyShaderTagIds[i]);
        }
        _context.DrawRenderers(_results,ref drawSetting,ref filterSetting);
    }

    /// <summary>
    /// Gizmos绘制
    /// </summary>
    partial void DrawGizmos()
    {
        if (Handles.ShouldRenderGizmos())
        {
            _context.DrawGizmos(_camera,GizmoSubset.PreImageEffects);
            _context.DrawGizmos(_camera,GizmoSubset.PostImageEffects);
        }
    }
    /// <summary>
    /// GUI绘制
    /// </summary>
    partial void DrawUGUI()
    {
        if (_camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
        }
    }


    private string SampleName { get; set; }
    /// <summary>
    /// 处理多相机
    /// </summary>
    partial void PerpareCamera()
    {
        Profiler.BeginSample("Editor Only");
        _buffer.name = SampleName = _camera.name;
        Profiler.EndSample();
    }
#else
    private const string SampleName = BufferName;
    #endif
    
    
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
        _buffer.ClearRenderTarget(true,false,Color.black); //清除渲染目标
        _context.SetupCameraProperties(_camera);//设置相机视图投影矩阵
        
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
