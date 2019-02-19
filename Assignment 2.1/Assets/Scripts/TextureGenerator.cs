using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureGenerator {
    public static Texture2D GenerateTexture(int terrianWidth, int terrianLength, Color[] color)
    {
        Texture2D texture = new Texture2D(terrianWidth, terrianLength);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(color);
        texture.Apply();
        return texture;
    }
}
