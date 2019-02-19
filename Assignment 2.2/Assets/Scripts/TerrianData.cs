using UnityEngine;

public struct TerrianData
{
    public readonly float[,] noise;
    public readonly Color[] color;

    public TerrianData(float[,] noise, Color[] color)
    {
        this.noise = noise;
        this.color = color;
    }
}
