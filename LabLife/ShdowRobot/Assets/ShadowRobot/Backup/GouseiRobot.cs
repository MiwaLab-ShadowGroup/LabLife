using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 合成
/// 上半身だけ別人
/// 1.完全に他人
/// 2.半分他人
/// 3.他人の割合を調整
/// </summary>
public class GouseiRobot : MonoBehaviour {

    public GameObject model;
    public int MaxLengthOfField;
    List<Human> List_Human;
    List<GameObject> List_Robot;
    public enum _rateMode 
    {
        none, rational, length,   //一定（50%)　距離に依存（遠いと相手の割合が多くなる）
    }
    _rateMode mode;
    CIPCReceiver cipc;

	// Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
	void Start () 
    {
        this.List_Human = new List<Human>();
        this.cipc = GameObject.FindGameObjectWithTag("CIPC").GetComponent<CIPCReceiver>();
        this.List_Robot = new List<GameObject>();
        this.mode = new _rateMode();
        this.mode = _rateMode.none;
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
                    this.List_Robot.Add(Instantiate(this.model));
                    this.List_Robot[i].transform.parent = this.transform;
                }

            }
            List<Human> list_robot = new List<Human>();
            list_robot = this.ChangePrayersBone(this.List_Human, this.mode);
            for (int i = 0; i < this.List_Robot.Count; i++)
            {
                this.List_Robot[i].GetComponent<ModelScript>().move(list_robot[i]);

            }
        }

    }

    
    List<Human> ChangePrayersBone(List<Human> list_human, _rateMode mode)
    {
        int count = list_human.Count - 1;
        List<Human> newList = new List<Human>();
        switch (mode)
        {
            case _rateMode.none: 
                for (int i = 0; i < count ; i++)
                {
                    newList.Add(this.ChangeData(list_human[i + 1], list_human[i]));       
                }
                newList.Add(this.ChangeData(list_human[0], list_human[count]));
                break;
            case _rateMode.rational:
                for (int i = 0; i < count; i++)
                {
                    newList.Add(this.ChangeData(list_human[i + 1], list_human[i], 0.5f));       
                    
                }
                newList.Add(this.ChangeData(list_human[0], list_human[count], 0.5f));
                break;
            case _rateMode.length: 
                for (int i = 0; i < count; i++)
                {
                    Vector3 interval = list_human[i + 1].bones[0].position - list_human[i].bones[0].position;
                    newList.Add(this.ChangeData(list_human[i + 1], list_human[i], 1 - (interval.magnitude / this.MaxLengthOfField) ));                          
                }
                Vector3 interval0 = list_human[0].bones[0].position - list_human[count].bones[0].position;
                newList.Add(this.ChangeData(list_human[0], list_human[count], 1 - (interval0.magnitude / this.MaxLengthOfField)));
                break;
        }
        

        //newList.Add(this.ChangeData(list_human[0], list_human[newList.Count - 1]));

        return newList;
    }
    //上半身下半身入れ替え
    Human ChangeData(Human topsData, Human bottomData)
    {
        Human human = new Human();
        human.bones = new Bone[topsData.bones.Length];
        
        for (int i = 0; i < 25; i++)
        {
            if (i < 12 || i > 20)
            {
                human.bones[i] = topsData.bones[i];
            }
            else
            {
                human.bones[i] = bottomData.bones[i];

            }
        }
        human.bones[0] = bottomData.bones[0];
        return human;
    }   
    //下半身自分，上半身割合指定で合成
    Human ChangeData(Human topsData, Human bottomData, float rate)
    {
        Human human = new Human();
        human.bones = new Bone[topsData.bones.Length];

        for (int i = 0; i < 25; i++)
        {
            if (i < 12 || i > 20)
            {
                human.bones[i] = topsData.bones[i];
                human.bones[i].quaternion = Quaternion.Lerp(topsData.bones[i].quaternion, bottomData.bones[i].quaternion, rate);
            }
            else
            {
                human.bones[i] = bottomData.bones[i];

            }
        }
        human.bones[0] = bottomData.bones[0];
        return human;
    }

    public void ChangeMode(int i)
    {
        switch (i)
        {
            case 0: this.mode = _rateMode.none; break;
            case 1: this.mode = _rateMode.rational; break;
            case 2: this.mode = _rateMode.length; break;

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
