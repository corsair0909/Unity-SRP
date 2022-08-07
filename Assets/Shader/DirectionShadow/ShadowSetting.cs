using UnityEngine;

[System.Serializable]
public class ShadowSetting
{
    [Min(0)]
    public float maxDistance = 100;
    

    public enum TextureSize
    {
        _256 = 256,
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096,
        _8192 = 8192,
    }

    //public TextureSize textureSize = TextureSize._1024;

    [System.Serializable]
    public struct Directional
    {
        public TextureSize atlasSize;
    }
    
    public Directional directional = new Directional()
        { atlasSize = TextureSize._1024 };
}
