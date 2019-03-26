using UnityEngine;

public static class TextureGenerator {
	public static Texture2D TextureFromColorMap(Color[] color, int terrianSize) {
        Texture2D texture = new Texture2D(terrianSize, terrianSize);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(color);
        texture.Apply();
		return texture;
	}
}
