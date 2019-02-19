using UnityEngine;

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    private int triangleIndex = 0;
    public Vector2[] uvs;

    public MeshData(int meshWidth, int meshLength)
    {
        vertices = new Vector3[meshWidth * meshLength];
        uvs = new Vector2[meshWidth * meshLength];
        triangles = new int[(meshWidth - 1) * (meshLength - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
