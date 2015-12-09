using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeSize : MonoBehaviour {

    public GameObject robotPrefab;

    GameObject robot;
    CIPCReceiver cipc;
    List<Human> List_Human;
    Human lastRoBot;
    Vector3[] lastVel;
    float[] AccMax;
    Vector3 baseSize;
	// Use this for initialization
	void Start () {
        this.robot = Instantiate(this.robotPrefab);
        this.robot.transform.parent = this.transform;
        this.cipc = GameObject.FindGameObjectWithTag("CIPC").GetComponent<CIPCReceiver>();

        this.List_Human = new List<Human>();
        this.lastRoBot = new Human();
        this.baseSize = new Vector3(5,5,5);
	}
	
	// Update is called once per frame
	void Update (){
        this.List_Human = this.cipc.List_Humans;
        if (this.List_Human.Count != 0)
        {
            Human Robot = this.List_Human[0];
            if (this.lastRoBot.bones!= null)
            {
                for (int i = 0; i < this.List_Human[0].bones.Length; i++)
                {
                    if (i == 7 || i == 11 || i == 14 || i == 18)
                    {
                        Vector3 vel = this.Velosity(Robot.bones[i].position, this.lastRoBot.bones[i].position);
                        if (this.lastVel != null)
                        {
                            Vector3 acc = this.Accessertate(vel, this.lastVel[i]);
                            float accSize = acc.magnitude ;
                            if (this.AccMax[i] < accSize || this.AccMax[i] == 0f) this.AccMax[i] = accSize;
                            //サイズチェンジ
                            Debug.Log((this.baseSize * (accSize / this.AccMax[i])).ToString());
                            this.robot.GetComponent<ModelScript>().ChangeSize(i, new Vector3(1f,1f,1f) +  this.baseSize * ( accSize/ this.AccMax[i]));
                        }

                        else
                        {
                            this.lastVel = new Vector3[this.List_Human[0].bones.Length];
                            this.AccMax = new float[this.List_Human[0].bones.Length];
                            for (int j = 0; j < this.AccMax.Length; j++) { this.AccMax[j] = 0f; }
                        }
                        this.lastVel[i] = vel;
                    }
                    
                    
                }
                this.robot.GetComponent<ModelScript>().move(Robot);
            }  
            this.lastRoBot = Robot;
        }
	}

    Vector3 Velosity(Vector3 Pos, Vector3 prePos)
    {
        Vector3 vel = (Pos - prePos) / Time.deltaTime;
        return vel;
    }
    Vector3 Accessertate(Vector3 vel, Vector3 preVel)
    {
        Vector3 acc = (vel - preVel) / Time.deltaTime;
        return acc;
    }
}
