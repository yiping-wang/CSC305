using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu()]
public class TextureAsset : UpdatableData
{
    [System.Serializable]
    public class Layer
    {
        public Texture2D texture;
        public Color tint;
        [Range(0, 1)]
        public float tintStrength;
        [Range(0, 1)]
        public float startLevels;
        [Range(0, 1)]
        public float blendStrength;
        public float textureScale;
    }

    const int textureSize = 512;
    const TextureFormat textureFormat = TextureFormat.RGB565;
    public int sunIntensity;
    public Layer[] layers;

    public void ApplyToMaterial(Material material)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].tint *= layers[i].tintStrength;
        }
        material.SetFloat("sunIntensity", sunIntensity);
        material.SetColorArray("baseTints", layers.Select(x => x.tint).ToArray());
        material.SetFloatArray("baseStartLevels", layers.Select(x => x.startLevels).ToArray());
        material.SetFloatArray("baseBlends", layers.Select(x => x.blendStrength).ToArray());
        material.SetFloatArray("baseColorStrength", layers.Select(x => x.tintStrength).ToArray());
        material.SetFloatArray("baseTextureScales", layers.Select(x => x.textureScale).ToArray());
        Texture2DArray texturesArray = GenerateTextureArray(layers.Select(x => x.texture).ToArray());
        material.SetTexture("baseTextures", texturesArray);
    }

    Texture2DArray GenerateTextureArray(Texture2D[] textures)
    {
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(), i);
        }
        textureArray.Apply();
        return textureArray;
    }

    public void UpdateLight(Material material, Vector3 pos, float intensity)
    {
        material.SetFloat("sunPosX", pos.x);
        material.SetFloat("sunPosY", pos.y);
        material.SetFloat("sunPosZ", pos.z);
        material.SetFloat("sunIntensity", intensity);
    }
}
