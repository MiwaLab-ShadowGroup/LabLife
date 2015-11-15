using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Particle : MonoBehaviour {

    public GameObject model;

    [Range(0.0f, 1.0f)]
    public float particleSpeed;

    Vector3[] particleTargetVec;
    ParticleSystem.Particle[] targetPaticle;

	// Use this for initialization
	void Start () {
        this.particleTargetVec = this.model.GetComponent<MeshFilter>().sharedMesh.vertices;
        this.targetPaticle = new ParticleSystem.Particle[this.particleTargetVec.Length];
	
	}
	
	// Update is called once per frame
	void Update () {
        //
        this.particleTargetVec = this.model.GetComponent<MeshFilter>().sharedMesh.vertices;
        

        for (int i = 0; i < this.targetPaticle.Length; i++)
        {
            this.particleTargetVec[i] = this.model.transform.TransformPoint(this.particleTargetVec[i]);
            this.targetPaticle[i].position = this.targetPaticle[i].position * (1f - this.particleSpeed) + this.particleTargetVec[i] * this.particleSpeed;

            this.targetPaticle[i].color = new Color(1f - this.particleTargetVec[i].x % 1f, 0.2f + this.particleTargetVec[i].y % 0.8f, 0.5f + particleTargetVec[i].z % 0.5f);
            this.targetPaticle[i].size = 0.05f;

            this.targetPaticle[i].lifetime = 10f;
            this.targetPaticle[i].startLifetime = 10f;
 

        }

        //
        GetComponent<ParticleSystem>().SetParticles(this.targetPaticle, this.targetPaticle.Length);
	
	}
}
