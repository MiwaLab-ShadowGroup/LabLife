using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 同じところから出す（ライトの投影対象分離）
///
/// </summary>

public class RobotSamePlace : MonoBehaviour {

    public GameObject robot;
    List<Human> List_Human;
    CIPCReceiver cipc;
    List<GameObject> List_Robot;


	void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        this.List_Human = new List<Human>();
        this.cipc = GameObject.FindGameObjectWithTag("CIPC").GetComponent<CIPCReceiver>();

        this.List_Robot = new List<GameObject>();

    }
	
	// Update is called once per frame
	void Update () 
    {
        this.List_Human = this.cipc.List_Humans;
        if (this.List_Human.Count != 0)
        {
            if (this.List_Robot.Count == 0 || this.List_Human.Count != this.List_Robot.Count)
            {
                for (int i = 0; i < this.List_Robot.Count; i++)
                {
                    Destroy(this.List_Robot[i]);
                }
                this.List_Robot.Clear();

                for (int i = 0; i < this.List_Human.Count; i++)
                {
                    this.List_Robot.Add(Instantiate(this.robot));
                    this.List_Robot[i].transform.parent = this.transform;
                }

            }
            for (int i = 0; i < this.List_Robot.Count; i++)
            {
                this.List_Robot[i].GetComponent<ModelScript>().move(this.cipc.List_Humans[i]);

            }
            this.RobotMotion(this.List_Human, this.List_Robot);
        }

	}

    void RobotMotion(List<Human> list_Human, List<GameObject> ListRobot)
    {
        for (int i = 0; i < ListRobot.Count; i++)
        {
            Quaternion quat = this.List_Human[i].bones[0].quaternion * Quaternion.AngleAxis(180, Vector3.up);
            ListRobot[i].GetComponent<ModelScript>().move(list_Human[i], this.List_Human[i].bones[0].position, quat);
        }
    }

    public void IsEnableChange(bool IsE)
    {
        if (this.List_Robot != null)
        {
            for (int i = 0; i < this.List_Robot.Count; i++)
            {
                this.List_Robot[i].SetActiveRecursively(IsE);

            }
        }
        
    }
}
