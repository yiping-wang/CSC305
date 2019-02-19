using UnityEngine;
using System.Collections;

public static class MeshGenerator {

	public static MeshData GenerateTerrainMesh(float[,] perlinNoise, float maxHeight, float waterLevel, int levelOfDetail) {
        float waterHeight = waterLevel * maxHeight;

        int terrianSize = perlinNoise.GetLength(0);
		float offsetX = (terrianSize - 1) / -2f;
        float offsetY = (terrianSize - 1) / 2f;

		int meshVertexStep = (levelOfDetail == 0)?1:levelOfDetail * 2;
		int numVerticesEachLine = (terrianSize - 1) / meshVertexStep + 1;

		MeshData meshData = new MeshData (numVerticesEachLine, numVerticesEachLine);
		int vertexIndex = 0;

        for (int y = 0; y < terrianSize; y += meshVertexStep) {
			for (int x = 0; x < terrianSize; x += meshVertexStep) {
                float regionHeight = perlinNoise[x, y] * maxHeight;
                if (regionHeight < waterHeight) {
                    regionHeight = waterHeight;
                }
                meshData.vertices [vertexIndex] = new Vector3 (offsetX + x, regionHeight, offsetY - y);
                meshData.uvs [vertexIndex] = new Vector2 (x / (float)terrianSize, y / (float)terrianSize);

                if (x < terrianSize - 1 && y < terrianSize - 1) {
					meshData.AddTriangle (vertexIndex, vertexIndex + numVerticesEachLine + 1, vertexIndex + numVerticesEachLine);
					meshData.AddTriangle (vertexIndex + numVerticesEachLine + 1, vertexIndex, vertexIndex + 1);
				}

				vertexIndex++;
			}
		}

		return meshData;

	}
}