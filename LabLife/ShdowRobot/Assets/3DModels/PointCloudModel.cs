using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointCloudModel : MonoBehaviour {

    List<ParticleSystem.Particle> particle;

	// Use this for initialization
	void Start () 
    {
        this.particle = new List<ParticleSystem.Particle>();
	}
	
	// Update is called once per frame
	void Update () 
    {
        GetComponent<ParticleSystem>().SetParticles(this.particle.ToArray(), this.particle.Count);
	}
}
