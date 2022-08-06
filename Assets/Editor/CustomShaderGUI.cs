using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomShaderGUI : ShaderGUI
{
    private MaterialEditor _editor;
    private Object[] _materials;
    private MaterialProperty[] _materialProperties;
    private bool showPresets;
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);
        this._editor = materialEditor;//材质编辑器，负责显示和编辑材质的对象
        this._materials = materialEditor.targets;//对正在编辑的材质的引用
        this._materialProperties = properties;//可编辑的属性数组
        EditorGUILayout.Space();
        //折叠按钮
        showPresets = EditorGUILayout.Foldout(showPresets, "Presets", true);
        if (showPresets)
        {
            OpaquePreset();
            ClipPreset();
            FadePreset();
            TransparentPreset();
        }

    }
    
    private bool Clipping
    {
        set => SetProperty("_Clipping", "CLIPPING", value);
    }
    private bool PermultiplyAlpha
    {
        set => SetProperty("PremulAlpha", "_PREMULTIPLY_ALPHA", value);
    }
    private BlendMode SrcBlend
    {
        set => SetProperty("_SrcBlend",(float)value);
    }
    private BlendMode DstBlend
    {
        set => SetProperty("_DstBlend",(float)value);
    }
    private bool ZWrite
    {
        set => SetProperty("_ZWrite",value?1f:0f);
    }

    RenderQueue RenderQueue
    {
        set
        {
            foreach (Material m in _materials)
            {
                m.renderQueue = (int)value;
            }
        }
    }
    
    void SetProperty(string name,string keyword,bool value)
    {
        if (SetProperty(name, value ? 1 : 0))
        {
            SetKeyWord(keyword,value);
        }
        
    }
    bool SetProperty(string name,float value)
    {
        //设置属性之前要在数组中找到属性
        FindProperty(name, _materialProperties).floatValue = value;
        MaterialProperty property = FindProperty(name, _materialProperties, false);
        if (property != null)
        {
            property.floatValue = value;
            return true;
        }

        return false;
    }
    void SetKeyWord(string keyword, bool enable)
    {
        if (enable)
        {
            foreach (Material m in _materials)    
            {
                m.EnableKeyword(keyword);
            }
        }
        else
        {
            foreach (Material m in _materials)
            {
                m.DisableKeyword(keyword);
            }
        }
    }

    bool presetButton(string name)
    {
        if (GUILayout.Button(name))
        {
            _editor.RegisterPropertyChangeUndo(name);
            return true;
        }
        return false;
    }

    void OpaquePreset()
    {
        if (presetButton("Opaque"))
        {
            Clipping = false;
            PermultiplyAlpha = false;
            SrcBlend = BlendMode.One;
            DstBlend = BlendMode.Zero;
            ZWrite = true;
            RenderQueue = RenderQueue.Geometry;
        }
    }
    void ClipPreset()
    {
        if (presetButton("Clip"))
        {
            Clipping = false;
            PermultiplyAlpha = true;
            SrcBlend = BlendMode.One;
            DstBlend = BlendMode.Zero;
            ZWrite = true;
            RenderQueue = RenderQueue.AlphaTest;
        }
    }
    void FadePreset()
    {
        if (presetButton("Fade"))
        {
            Clipping = false;
            PermultiplyAlpha = true;
            SrcBlend = BlendMode.SrcAlpha;
            DstBlend = BlendMode.OneMinusSrcAlpha;
            ZWrite = false;
            RenderQueue = RenderQueue.Transparent;
        }
    }
    void TransparentPreset()
    {
        if (presetButton("Transparent"))
        {
            Clipping = false;
            PermultiplyAlpha = true;
            SrcBlend = BlendMode.One;
            DstBlend = BlendMode.OneMinusSrcAlpha;
            ZWrite = false;
            RenderQueue = RenderQueue.Transparent;
        }
    }

}
