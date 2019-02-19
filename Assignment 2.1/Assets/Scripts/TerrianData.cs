using UnityEngine;

public struct TerrianData
{
    public readonly float[,] height;
    public readonly Color[] color;

    public TerrianData(float[,] height, Color[] color)
    {
        this.height = height;
        this.color = color;
    }
}