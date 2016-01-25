using UnityEngine;
using System.Collections;

public class Cylinder : MonoBehaviour {

	// Use this for initialization
	void Start () {

        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.material.color = new Color(1.0f, 1.0f, 0);

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
