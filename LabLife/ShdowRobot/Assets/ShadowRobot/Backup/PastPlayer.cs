using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 時間遅れ影　
/// 出す場所：　同じところから（ライト調整必要）　相澤モデルでぽししょん）
/// 動き：そのまま　今の動きをたす
/// </summary>
public class PastPlayer : MonoBehaviour {

    //public GameObject model;
    public GameObject robot_model;
    List<Human> List_Human;
    List<Human> List_PastHuman;
    CIPCReceiver cipc;
    int interval;
    GameObject robot;
    AizawaModel aizawaModel;
    public bool IsAizawa;
    bool IsSamePose;
    float rate;


	// Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        Application.targetFrameRate = 60;
        Debug.Log("Generator:TimeDelay");        
        this.List_Human = new List<Human>();
        this.List_PastHuman = new List<Human>();
        this.cipc = GameObject.FindGameObjectWithTag("CIPC").GetComponent<CIPCReceiver>();
        //5s遅れ
        this.interval = 5;
        this.robot = Instantiate(this.robot_model);
        this.robot.transform.parent = this.transform;
        this.IsSamePose = true;
        this.rate = 0.5f;
        //相澤モデル
        //this.IsAizawa = true;
        if (this.IsAizawa)
        {
            this.aizawaModel = this.GetComponent<AizawaModel>();
            this.aizawaModel.Init();
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        if (this.cipc.List_Humans.Count != 0)
        {
            this.List_Human = this.cipc.List_Humans;
            //取得データのうちiD 0の人の情報を保存
            this.List_PastHuman.Add(this.List_Human[0]);
           // Debug.Log("Generator:TimeDelayadd");        

            //十分な保存データがあるときはロボットを表示
            if (this.List_PastHuman.Count > (this.interval * Application.targetFrameRate))
            {
                //float i = (this.interval * Application.targetFrameRate);
                //Debug.Log("Generator:" + this.List_PastHuman.Count.ToString() + ":" + i.ToString()) ;        

                Human robotdata = new Human();
                //古いデータを使って表示
                //ポーズ変更
                if (!this.IsSamePose)
                {
                    robotdata = this.MixBone(this.List_PastHuman[0], this.List_PastHuman[this.List_PastHuman.Count - 1], this.rate);
                }
                else robotdata = this.List_PastHuman[0];
                //相澤モデル
                if (this.IsAizawa)
                {
                    Vector3 vector = this.aizawaModel.AizawaModel3D(this.List_Human[0], 0);
                    this.robot.GetComponent<ModelScript>().move(robotdata, vector);
                }               
                //同じとこから出す
                else
                {
                    Vector3 vector = this.List_PastHuman[this.List_PastHuman.Count - 1].bones[0].position;
                    this.robot.GetComponent<ModelScript>().move(robotdata, vector);
                }

                //データ消去
                this.List_PastHuman.RemoveAt(0);
            }
        }	
	}

    public void ChangeInterval(int interval)
    {
        this.List_Human = this.ChangeInterval(interval, this.List_Human);
    }
    List<Human> ChangeInterval(int interval, List<Human> orgList)
    {
        List<Human> list_human = new List<Human>();
        if (this.interval < interval)
        {
            int count = (this.interval - interval) * Application.targetFrameRate;
            for (int i = count; i < list_human.Count; i++)
            {
                list_human.Add(orgList[i]);
            }
            return list_human;
        }
        else return orgList;
       
       
    }

    public void ChangeRate(float f)
    {
        this.rate = f;
    }
    public void SamePose(bool Is)
    {
        this.IsSamePose = Is;
    }
    //動きの合成
    Human MixBone(Human human0, Human human1, float range)
    {
        Human newHuman = new Human();
        newHuman.bones = new Bone[human1.bones.Length];
        for (int i = 0; i < human0.bones.Length; i++)
        {
            Bone bone = new Bone();
            if (i != 3 &&i != 21 &&i != 22 &&i != 23 &&i != 24 &&i != 15 &&i != 19 )
            {
                
                bone.quaternion = Quaternion.Lerp(human0.bones[i].quaternion, human1.bones[i].quaternion, range);
                
            }else
                bone.quaternion =  human0.bones[i].quaternion;

            newHuman.bones[i] = bone;
        }

        return newHuman;
    }

    public void IsEnableChange(bool IsE)
    {
        if(this.robot != null)
            this.robot.SetActiveRecursively(IsE);
    }
}
