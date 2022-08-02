
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class PerObjectMaterialProp : MonoBehaviour
{
    private static int BaseColorID = Shader.PropertyToID("_BaseColor");
    [SerializeField]
    Color baseColor = Color.white;

    private static MaterialPropertyBlock _block;

    private void Awake()
    {
        OnValidate();
    }

    private void OnValidate()
    {
        if (_block==null)
        {
            _block = new MaterialPropertyBlock();
        }
        _block.SetColor(BaseColorID,baseColor);
        GetComponent<Renderer>().SetPropertyBlock(_block);
    }
}
