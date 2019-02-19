using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] terrianHeight, float maxHeight, float waterLevel, int meshSimplificationFactor)
    {
        int width = terrianHeight.GetLength(0);
        int length = terrianHeight.GetLength(1);
        int vertexIndex = 0;
        int vertexIndexStep = (meshSimplificationFactor <= 1) ? 1 : (int) Mathf.Pow(5f, meshSimplificationFactor - 1);
        int vertexNumber = (width - 1) / vertexIndexStep + 1;
        /* Make mesh center at the screen. */
        float centerX = (width - 1) / -2f;
        float centerY = (length - 1) / 2f;
        float waterHeight = waterLevel * maxHeight;

        MeshData meshData = new MeshData(vertexNumber, vertexNumber);

        for (int y = 0; y < length; y+=vertexIndexStep)
        {
            for (int x = 0; x < width; x+=vertexIndexStep)
            {
                float regionHeight = terrianHeight[x, y] * maxHeight;
                if (regionHeight < waterHeight)
                {
                    regionHeight = waterHeight;
                }
                meshData.vertices[vertexIndex] = new Vector3(centerX + x, regionHeight, centerY - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)length);
                if (x < width - 1 && y < length - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + vertexNumber + 1, vertexIndex + vertexNumber);
                    meshData.AddTriangle(vertexIndex + vertexNumber + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }
        return meshData;
    }
}
