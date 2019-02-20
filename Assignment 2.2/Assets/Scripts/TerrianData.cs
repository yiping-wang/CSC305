using UnityEngine;

public struct TerrianData
{
    public readonly float[,] noise;

    public TerrianData(float[,] noise)
    {
        this.noise = noise;
    }
}
