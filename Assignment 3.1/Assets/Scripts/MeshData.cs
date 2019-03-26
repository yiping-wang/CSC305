using UnityEngine;

public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

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

    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int numTriangles = triangles.Length / 3;
        for (int i = 0; i < numTriangles; i++)
        {
            int vertexA = triangles[i * 3];
            int vertexB = triangles[i * 3 + 1];
            int vertexC = triangles[i * 3 + 2];

            Vector3 AB = vertices[vertexB] - vertices[vertexA];
            Vector3 BC = vertices[vertexC] - vertices[vertexB];
            Vector3 vertexNormal = Vector3.Cross(AB, BC).normalized;

            vertexNormals[vertexA] += vertexNormal;
            vertexNormals[vertexB] += vertexNormal;
            vertexNormals[vertexC] += vertexNormal;
        }

        foreach (Vector3 vertexNormal in vertexNormals)
        {
            vertexNormal.Normalize();
        }

        return vertexNormals;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.normals = CalculateNormals();
        return mesh;
    }
}

