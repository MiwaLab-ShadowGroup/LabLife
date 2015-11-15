using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AizawaRobot : MonoBehaviour {

    //public 
    public GameObject robot;

    CIPCReceiver CIPC;

    public enum _actMode
    {
        bone, pos,
    }
    public enum _demMode
    {
        twod, threed,
    }
    public enum _timing
    {
        alltime, moment, stay,
    }
    _timing timeing;
    _actMode actMode;
    _demMode demMode;

    Aizawa aizawa;
    int BoneNum;
    List<Human> List_Human;
	// Use this for initialization
	void Start () 
    {
        this.aizawa = new Aizawa();
        this.List_Human = new List<Human>();
        this.robot = Instantiate(this.robot);
        this.CIPC = GameObject.FindGameObjectWithTag("CIPC").GetComponent<CIPCReceiver>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	   this.List_Human = this.CIPC.List_Humans;
       if (this.List_Human.Count != 0)
       {
           Vector3 vector = new Vector3(0, 0, 0);
           Quaternion quat = new Quaternion();
           switch (this.demMode)
           {
               case _demMode.twod: vector = this.RobotAction2D(this.List_Human, this.BoneNum ); break;
               case _demMode.threed: vector = this.RobotAction3D(this.List_Human, this.BoneNum); break;
           }
           quat = this.List_Human[0].bones[0].quaternion;
           this.robot.GetComponent<ModelScript>().move(vector,quat);
       }
	}

    Vector3 RobotAction3D(List<Human> list_human, int bonenum)
    {
        Vector3 vector = new Vector3(0, 0, 0);
        for (int i = 0; i < list_human.Count; i++)
        {
            vector += this.aizawa.AizawaModel3D(list_human[i], bonenum);
        }
        vector /= list_human.Count;
        return vector;
    }
    Vector3 RobotAction2D(List<Human> list_human, int bonenum)
    {
        Vector3 vector = new Vector3(0, 0, 0);
        for (int i = 0; i < list_human.Count; i++)
        {
            vector += this.aizawa.AizawaModel2D(list_human[i], bonenum);
        }
        vector /= list_human.Count;
        return vector;

    }

    
}
