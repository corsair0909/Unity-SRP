using UnityEngine;
using UnityEngine.Rendering;

public class Shadows
{
    //Buffer name
    private const string bufferName = "Shadows";
    //产生阴影的光的最大数量
    private const int maxShadowedDirectionalLightCount = 1;

    //当前产生阴影的灯光
    private int shadowedDirectionalLightCount;

    private static int dirShadowAtlasID = Shader.PropertyToID("_DirectionalShadowAtlas");

    struct shadowedDirectionalLight
    {
        public int visibleLightIndex;
    }
    
    //不知道产生阴影的可见光是哪一束光，用数组追逐
    private shadowedDirectionalLight[] _shadowedDirectionalLights =
        new shadowedDirectionalLight[maxShadowedDirectionalLightCount];

    //CommandBuffer 实例
    private CommandBuffer _buffer = new CommandBuffer()
    {
        name = bufferName
    };
    
    // 上下文
    private ScriptableRenderContext _context;
    //裁剪结果
    private CullingResults _cullingResults;
    //阴影设置实例
    private ShadowSetting _shadowSetting;

    //创建阴影图集
    public void Render()
    {
        //Debug.Log(shadowedDirectionalLightCount);
        if (shadowedDirectionalLightCount>0)
        {
            RenderDirectionalShadows();
        }
    }

    void RenderDirectionalShadows()
    {
        int atlasSize = (int)_shadowSetting.directional.atlasSize;
        //默认情况会生成ARGB纹理，而需要ShadowMap纹理
        _buffer.GetTemporaryRT(dirShadowAtlasID,atlasSize,atlasSize,32,FilterMode.Bilinear,
            RenderTextureFormat.Shadowmap);
        //让GPU渲染到纹理上而不是相机目标，表示渲染纹理，如何加载和存储
        _buffer.SetRenderTarget(dirShadowAtlasID,
            RenderBufferLoadAction.DontCare,RenderBufferStoreAction.Store);
        _buffer.ClearRenderTarget(true,false,Color.clear);
        _buffer.BeginSample(bufferName);
        ExcuteBuffer();
        for (int i = 0; i < shadowedDirectionalLightCount; i++)
        {
            RenderDirectionalShadows(i,atlasSize);
        }
        _buffer.EndSample(bufferName);
        ExcuteBuffer();
    }

    void RenderDirectionalShadows(int index, int tileSize)
    {
        shadowedDirectionalLight lights = _shadowedDirectionalLights[index];
        var shadowSetting = new ShadowDrawingSettings(_cullingResults,lights.visibleLightIndex);
        _cullingResults.ComputeDirectionalShadowMatricesAndCullingPrimitives(
            lights.visibleLightIndex, 0, 1, Vector3.zero, tileSize, 0f,
            out Matrix4x4 viewMatrix, out Matrix4x4 projectionMatrix, out ShadowSplitData splitData
        );
        shadowSetting.splitData = splitData;
        _buffer.SetViewProjectionMatrices(viewMatrix,projectionMatrix);
        ExcuteBuffer();
        _context.DrawShadows(ref shadowSetting);
    }

    public void Setup(ScriptableRenderContext context,CullingResults cullingResults,ShadowSetting shadowSetting)
    {
        this._context = context;
        this._cullingResults = cullingResults;
        this._shadowSetting = shadowSetting;
        shadowedDirectionalLightCount = 0;
    }

    public void Cleanup()
    {
        if (shadowedDirectionalLightCount > 0)
        {
            _buffer.ReleaseTemporaryRT(dirShadowAtlasID);
            ExcuteBuffer();
        }
    }

    public void ExcuteBuffer()
    {
        _context.ExecuteCommandBuffer(_buffer);
        _buffer.Clear();
    }

    /// <summary>
    /// 阴影需要留给开启阴影的灯光，阴影模式不为none且阴影强度大于0的光源会有阴影
    /// 获得可见光索引来检查阴影影响范围，返回边界是否存在，若不是则没有阴影可渲染
    /// </summary>
    /// <param name="light"></param>
    /// <param name="visibleLightIndex"></param>
    /// && _cullingResults.GetShadowCasterBounds(visibleLightIndex,out  Bounds b)
    public void ReserveDirectionalShadows(Light light, int visibleLightIndex)
    {
        if (shadowedDirectionalLightCount<maxShadowedDirectionalLightCount
            && light.shadows != LightShadows.None && light.shadowStrength > 0f
            )
        {
            _shadowedDirectionalLights[shadowedDirectionalLightCount++] =
                new shadowedDirectionalLight() { visibleLightIndex = visibleLightIndex };
            Debug.Log(shadowedDirectionalLightCount);
        }
    }
}
