using UnityEngine;
using System.Collections;

public class Shape : MonoBehaviour {

    public Mesh mesh;
    public int[] indices;
    public Vector3[] Verices;
    public Vector2[] UvMaps;

	// Use this for initialization
	void Start ()
    {
        this.mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = this.mesh;

        this.mesh.vertices = this.Verices;
        this.mesh.triangles = this.indices;
        this.mesh.uv = this.UvMaps;
        this.mesh.RecalculateNormals();
        this.mesh.RecalculateBounds();
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	
	}
}
