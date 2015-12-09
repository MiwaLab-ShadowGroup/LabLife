using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TetrahedronScript : MonoBehaviour
{
    public Mesh mesh;
    public int[] indices;
    public Vector3[] Vertices;
    public Vector2[] UvMaps;
    // Use this for initialization
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = this.mesh;

        mesh.vertices = Vertices;
        mesh.triangles = indices;
        mesh.uv = UvMaps;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
