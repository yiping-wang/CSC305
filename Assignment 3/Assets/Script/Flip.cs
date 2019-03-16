using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flip : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        int[] indices = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector2[] UVs = mesh.uv;
        for (int i = 0; i < vertices.Length; i++) 
        {
            vertices[i].y = vertices[i].y / 8f;
        }
        mesh.vertices = vertices;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
