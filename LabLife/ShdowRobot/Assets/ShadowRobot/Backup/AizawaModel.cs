using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

/// <summary>
/// 相澤モデル
/// 1.同じ姿勢で移動
/// 2.位置だけ移動
/// 3.目標距離変化
///   1.ランダム
///   1.常に変化
///   2.距離に達したら変化
///   3.一定時間維持したら変化
/// </summary>
public class AizawaModel : MonoBehaviour {

    public GameObject modelofRobot;
    public List<Human> List_Humans;
    public int targetBoneNum;
    #region
    public enum _actMode
    {
        bone, pos,
    }
    public enum _demMode
    {
        twod,threed,
    }
    public enum _timing
    {
        alltime, moment, stay,
    }
    _timing timeing;
    _actMode actMode;
    _demMode demMode;
    bool IsChangeInterval;
    bool IsAllTimeChange;
    bool IsRadam;
    float KeepTime;
    float InterValKeepedTime;
    float TargetInterval;
    GameObject model;
    CIPCReceiver cipc;
    Human robotBone;

    int[] tree;
    #endregion
    //uman target;
    //public List<Human> List_Humans;

    System.Threading.Timer timer;
    Time time;

    //パラメータ等
    #region
    int datalength;
    int timerIntervalMillsec;
    Vector3[] PositionofRobot;
    Vector3[] PositionofHuman;
    Vector3 Vec;
    float[] alfa;
    float initLength;
    int S;
    float[] Ka_List;
    float[] Ka;
    float[] Kb;
    int secondPosition;
    int secondStep;
    #endregion

    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    // Use this for initialization
	void Start () 
    {
        this.cipc = GameObject.FindGameObjectWithTag("CIPC").GetComponent<CIPCReceiver>();
        this.robotBone = new Human();

        this.List_Humans = new List<Human>();
        this.tree = this.MakeParent();

        Debug.Log("Generator:AizawaModel");
        this.model = Instantiate(this.modelofRobot);
        //this.model = this.modelofRobot;

        this.model.transform.parent = this.transform;
        this.IsRadam = false;
        this.IsChangeInterval = false;
        this.IsAllTimeChange = false;

        this.actMode = new _actMode();
        this.demMode = new _demMode();
        this.actMode = _actMode.bone;
        this.demMode = _demMode.twod;

        this.Init();
        this.model.transform.position = this.PositionofRobot[1];

        this.Ka_List = this.MakeKa(25);
        this.KeepTime = 3f;
	}
	
	// Update is called once per frame
	void Update () 
    {
       
        //List_Humanにrobotactionを追加
        this.List_Humans = this.cipc.List_Humans;
        //this.RobotAllBone();
        
        switch (this.demMode)
        {
            case _demMode.twod: this.RobotAction2D(this.targetBoneNum); break;
            case _demMode.threed: this.RobotAction3D(this.targetBoneNum); break;
        }
        
         
	}

    //影ロボットの動き
    //相澤モデル
    public void Init()
    {
        this.timerIntervalMillsec = 100;

        //データ格納配列
        this.datalength = 3;
        this.PositionofHuman = new Vector3[this.datalength];
        this.PositionofRobot = new Vector3[this.datalength];
        
        //影ロボット最初の移動距離[m]
        this.initLength = 0.1f;
        this.Vec = Vector3.right;
        for (int i = 0; i < this.PositionofRobot.Length; i++)
        {
            this.PositionofRobot[i] = new Vector3(10,0,0) - this.Vec * this.initLength * i * Time.deltaTime;
            this.PositionofHuman[i] = new Vector3(0,0,0)  + this.Vec * this.initLength * i * Time.deltaTime;
        }
        //this.model.transform.position = this.PositionofRobot[1];
        //定数初期値
        this.Ka = new float[2];
        this.Kb = new float[2];
        this.Ka[0] = 1;
        this.Ka[1] = 1;
        this.Kb[0] = 1;
        this.Kb[1] = 1;
        this.S = 1;

        this.TargetInterval = Mathf.Sqrt((Kb[0] + Kb[1]) / (Ka[0] + Ka[1]));
        this.robotBone = new Human();
        this.robotBone.bones = new Bone[25];
        Bone bone = new Bone();
        this.robotBone.bones[0] = bone;
        this.robotBone.bones[0].position = new Vector3(10, 0, 0);
    }


    void RobotAction3D(int boneNum)
    {
        try
        {
            
            if (this.cipc.List_Humans.Count != 0)
            {
                if (this.IsChangeInterval)
                {
                    switch (this.timeing)
                    {
                        case _timing.alltime: this.ChangeKa(this.IsRadam); break;
                        case _timing.moment: this.ChangeKa(this.List_Humans[0], true, this.IsRadam); break;
                        case _timing.stay: this.ChangeKa(this.List_Humans[0], false, this.IsRadam); break;

                    }
                }
                Vector3 vector = this.AizawaModel3D(boneNum);
                this.robotBone = this.List_Humans[0];
                //Debug.Log("aizawa0");
                //モデル操作
                if (boneNum != 0)
                {
                    switch (this.actMode)
                    {
                        case _actMode.bone:
                            this.model.GetComponent<ModelScript>().move(this.robotBone, vector);
                            break;
                        case _actMode.pos:
                            Quaternion quat = this.robotBone.bones[0].quaternion * Quaternion.AngleAxis(180, Vector3.up);                
                            this.model.GetComponent<ModelScript>().move(vector, quat);
                            break;
                    }
                }
                else
                {
                    switch (this.actMode)
                    {
                        case _actMode.bone:
                            Quaternion quat = this.robotBone.bones[0].quaternion * Quaternion.AngleAxis(180, Vector3.up);
                            this.model.GetComponent<ModelScript>().move(this.robotBone, vector, quat);
                            break;
                        case _actMode.pos:
                            Quaternion quat1 = this.robotBone.bones[0].quaternion * Quaternion.AngleAxis(180, Vector3.up);
                            this.model.GetComponent<ModelScript>().move(vector, quat1);
                            break;
                    }
                }
            }
            
        }
        catch
        {
            Debug.Log("Error:RobotAction");
        }
        
    }
    void RobotAction2D(int boneNum)
    {
        try
        {
            //Debug.Log(this.cipc.List_Humans.Count.ToString());
            //データ取得
            if (this.List_Humans.Count != 0)
            {
                if (this.IsChangeInterval)
                {
                    switch (this.timeing)
                    {
                        case _timing.alltime: this.ChangeKa(this.IsRadam); break;
                        case _timing.moment: this.ChangeKa(this.List_Humans[0], true, this.IsRadam) ;break;
                        case _timing.stay: this.ChangeKa(this.List_Humans[0], false , this.IsRadam); break;

                    }                   
                }
                Vector3 vector = this.AizawaModel2D(boneNum);
                this.robotBone = this.List_Humans[0];
                //モデル操作
                if (boneNum != 0)
                {
                    switch (this.actMode)
                    {
                        case _actMode.bone:
                            this.model.GetComponent<ModelScript>().moveAndTransToTargetPosition(this.robotBone, boneNum, vector, true);
                            break;
                        case _actMode.pos:
                            this.model.GetComponent<ModelScript>().moveAndTransToTargetPosition(this.robotBone, boneNum, vector, false);

                            break;
                    }
                }
                else
                {
                    switch (this.actMode)
                    {
                        case _actMode.bone:
                            Quaternion quat = this.robotBone.bones[0].quaternion * Quaternion.AngleAxis(180, Vector3.up);
                            this.model.GetComponent<ModelScript>().move(this.robotBone, vector, quat);
                            break;
                        case _actMode.pos:
                            Quaternion quat1 = this.robotBone.bones[0].quaternion * Quaternion.AngleAxis(180 ,Vector3.up);
                            this.model.GetComponent<ModelScript>().move(vector, quat1);
                            break;
                    }
                }
                
                
            }
        }
        catch
        {
            Debug.Log("Error:RobotAction");
        }

    }
    void RobotAllBone()
    {
        try
        {
            if (this.cipc.List_Humans.Count != 0)
            {
                Human orgHuman = new Human();
                orgHuman.bones = new Bone[this.List_Humans[0].bones.Length];
                for (int i = 0; i < this.List_Humans[0].bones.Length; i++)
                {
                    Bone bone  = new Bone();
                    this.Ka[0] = this.Ka_List[i];
                    this.Ka[1] = this.Ka_List[i];
                    bone.position = this.AizawaModel3D(i);
                    orgHuman.bones[i] = bone;
                }
                this.robotBone = this.MakeQuaternion(orgHuman, this.tree);
                this.model.GetComponent<ModelScript>().moveByMakedQuat(this.robotBone);
            }
            
        }
       catch
       { 
            Debug.Log("Error:RobotAllBone");
       }
        
    }

    Vector3 AizawaModel3D(int boneNum)
    {
        Human localRobot  = this.List_Humans[0];

        //データ入れ替え
        this.ReplaceData(this.datalength, ref this.PositionofRobot);
        this.ReplaceData(this.datalength, ref this.PositionofHuman);
        this.AddNewDataToArray(this.cipc.List_Humans[0].bones[boneNum].position, this.datalength, ref this.PositionofHuman);
        
        //速度算出
        Vector3 velocityofRobot = (this.PositionofRobot[1] - this.PositionofRobot[0]) / Time.deltaTime;
        Vector3 velocityofHuman = (this.PositionofHuman[1] - this.PositionofHuman[0]) / Time.deltaTime;

        //移動ベクトル
        this.Vec = this.PositionofHuman[1] - this.PositionofRobot[1];
        this.Vec /= this.Vec.magnitude;

        //計算
        //自分の位置予測
        Vector3 nextRobotPosition = PositionofRobot[0] + 2 * this.S * velocityofRobot * Time.deltaTime;
        //相手の位置予測
        Vector3 nextHumanPosition = this.PositionofHuman[1] + (this.PositionofHuman[1] - this.PositionofHuman[0]) * Time.deltaTime;
        //予測感覚
        Vector3 interval = nextRobotPosition - nextHumanPosition;
        //評価関数
        float alfa = (this.Ka[0] * interval.magnitude - this.Ka[1] / interval.magnitude);

        //移動
        Vector3 vector = this.PositionofRobot[1] + new Vector3(alfa * this.Vec.x, 0, alfa * this.Vec.z) * Time.deltaTime;
        vector = new Vector3(vector.x, localRobot.bones[boneNum].position.y, vector.z);
        //this.robotBone.bones[0].position = vector;
        //算出したデータを保存
        this.AddNewDataToArray(vector, this.datalength, ref this.PositionofRobot);

        return vector;
    }
    Vector3 AizawaModel2D(int boneNum)
    {
        Human localRobot = this.List_Humans[0];


        //データ入れ替え
        this.ReplaceData(this.datalength, ref this.PositionofRobot);
        this.ReplaceData(this.datalength, ref this.PositionofHuman);
        this.AddNewDataToArray(this.cipc.List_Humans[0].bones[boneNum].position, this.datalength, ref this.PositionofHuman);

        //速度算出
        Vector3 velocityofRobot = (this.PositionofRobot[1] - this.PositionofRobot[0]) / Time.deltaTime;
        Vector3 velocityofHuman = (this.PositionofHuman[1] - this.PositionofHuman[0]) / Time.deltaTime;

        //移動ベクトル
        this.Vec = new Vector3(this.PositionofHuman[1].x - this.PositionofRobot[1].x, 0, 0);
        this.Vec /= this.Vec.magnitude;

        //計算
        //自分の位置予測
        Vector3 nextRobotPosition = PositionofRobot[0] + 2 * this.S * velocityofRobot * Time.deltaTime;
        //相手の位置予測
        Vector3 nextHumanPosition = this.PositionofHuman[1] + (this.PositionofHuman[1] - this.PositionofHuman[0]) * Time.deltaTime;
        //予測感覚
        float interval = nextRobotPosition.x - nextHumanPosition.x;
        //評価関数
        float alfa = (this.Ka[0] * interval - this.Ka[1] / interval);

        //移動
        Vector3 vector = this.PositionofRobot[1] + new Vector3(alfa * this.Vec.x, 0, 0) * Time.deltaTime;
        vector = new Vector3(vector.x, localRobot.bones[boneNum].position.y, this.PositionofHuman[1].z);
        //this.robotBone.bones[0].position = vector;

        //算出したデータを保存
        this.AddNewDataToArray(vector, this.datalength, ref this.PositionofRobot);

        return vector;
    }

    public Vector3 AizawaModel3D(Human human, int boneNum)
    {
        Human robot = human;
        Human player = human;
        //データ入れ替え
        this.ReplaceData(this.datalength, ref this.PositionofRobot);
        this.ReplaceData(this.datalength, ref this.PositionofHuman);
        this.AddNewDataToArray(player.bones[boneNum].position, this.datalength, ref this.PositionofHuman);

        //速度算出
        Vector3 velocityofRobot = (this.PositionofRobot[1] - this.PositionofRobot[0]) / Time.deltaTime;
        Vector3 velocityofHuman = (this.PositionofHuman[1] - this.PositionofHuman[0]) / Time.deltaTime;

        //移動ベクトル
        this.Vec = this.PositionofHuman[1] - this.PositionofRobot[1];
        this.Vec /= this.Vec.magnitude;

        //計算
        //自分の位置予測
        Vector3 nextRobotPosition = PositionofRobot[0] + 2 * this.S * velocityofRobot * Time.deltaTime;
        //相手の位置予測
        Vector3 nextHumanPosition = this.PositionofHuman[1] + (this.PositionofHuman[1] - this.PositionofHuman[0]) * Time.deltaTime;
        //予測感覚
        Vector3 interval = nextRobotPosition - nextHumanPosition;
        //評価関数
        float alfa = (this.Ka[0] * interval.magnitude - this.Ka[1] / interval.magnitude);

        //移動
        Vector3 vector = this.PositionofRobot[1] + new Vector3(alfa * this.Vec.x, 0, alfa * this.Vec.z) * Time.deltaTime;
        vector = new Vector3(vector.x, robot.bones[boneNum].position.y, vector.z);
        //this.robotBone.bones[0].position = vector;
        //算出したデータを保存
        this.AddNewDataToArray(vector, this.datalength, ref this.PositionofRobot);

        return vector;
    }
    public Vector3 AizawaModel2D(Human human, int boneNum)
    {
        Human robot = human;
        Human player = human;

        //データ入れ替え
        this.ReplaceData(this.datalength, ref this.PositionofRobot);
        this.ReplaceData(this.datalength, ref this.PositionofHuman);
        this.AddNewDataToArray(player.bones[boneNum].position, this.datalength, ref this.PositionofHuman);

        //速度算出
        Vector3 velocityofRobot = (this.PositionofRobot[1] - this.PositionofRobot[0]) / Time.deltaTime;
        Vector3 velocityofHuman = (this.PositionofHuman[1] - this.PositionofHuman[0]) / Time.deltaTime;

        //移動ベクトル
        this.Vec = new Vector3(this.PositionofHuman[1].x - this.PositionofRobot[1].x, 0, 0);
        this.Vec /= this.Vec.magnitude;

        //計算
        //自分の位置予測
        Vector3 nextRobotPosition = PositionofRobot[0] + 2 * this.S * velocityofRobot * Time.deltaTime;
        //相手の位置予測
        Vector3 nextHumanPosition = this.PositionofHuman[1] + (this.PositionofHuman[1] - this.PositionofHuman[0]) * Time.deltaTime;
        //予測感覚
        float interval = nextRobotPosition.x - nextHumanPosition.x;
        //評価関数
        float alfa = (this.Ka[0] * interval - this.Ka[1] / interval);

        //移動
        Vector3 vector = this.PositionofRobot[1] + new Vector3(alfa * this.Vec.x, 0, 0) * Time.deltaTime;
        vector = new Vector3(vector.x, robot.bones[boneNum].position.y, this.PositionofHuman[1].z);
        //this.robotBone.bones[0].position = vector;

        //算出したデータを保存
        this.AddNewDataToArray(vector, this.datalength, ref this.PositionofRobot);

        return vector;
    }

    void ReplaceData(int lengthofArray, ref Vector3[] array)
    {
        //n+1（最後）のデータは計算後に出す
        //n（最後から2つ目）が取得データ
        for (int i = 0; i < lengthofArray - 1; i++)
        {
            array[i] = array[i + 1];
        }
        //array[lengthofArray-1] = newData;
    }
    void AddNewDataToArray(Vector3 newData, int lengthofArray, ref Vector3[] array)
    {
        array[lengthofArray - 1] = newData;
    }
    void OnAppLicatinQuit()
    {
        //this.timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        this.timer.Dispose();
    }

    Human MakeQuaternion(Human human, int[] tree)
    {
        Human robot = new Human();
        robot.bones = new Bone[human.bones.Length];

        robot.bones[0] = human.bones[0];    
        for (int i = 1; i < this.cipc.List_Humans[0].bones.Length; i++)
        {
            Bone bone = new Bone();
            if (this.tree[i] != 0)
            {
                bone.quaternion = Quaternion.FromToRotation(Vector3.up, this.List_Humans[0].bones[tree[i]].position - this.List_Humans[0].bones[i].position);
                //bone.quaternion = Quaternion.FromToRotation(robot.bones[0].position, this.List_Humans[0].bones[tree[i]].position - this.List_Humans[0].bones[i].position);       
                //bone.quaternion = Quaternion.FromToRotation(this.List_Humans[0].bones[i].position, this.List_Humans[0].bones[tree[i]].position - this.List_Humans[0].bones[i].position);
            }
            else bone.quaternion = new Quaternion(0, 0, 0, 0);
            
            robot.bones[i] = bone;
            Debug.Log("make:" + i.ToString() + robot.bones[i].quaternion.ToString() + this.cipc.List_Humans[0].bones[i].quaternion.ToString() + this.List_Humans[0].bones[i].quaternion.ToString());

        }
        for (int i = 1; i < this.cipc.List_Humans[0].bones.Length; i++)
        {
            if (this.tree[i] != 0)
                robot.bones[i].quaternion = Quaternion.Lerp(human.bones[i].quaternion, robot.bones[i].quaternion, 0.5f);
        }
        

        return robot;
    }
    int[] MakeParent()
    {
                     //0  1   2  3  4   5  6  7  8   9 10  11 12  13  14  15  16 17  18  19  20 21 22 23 24   
      //int[] tree = { 0, 0, 20, 2, 20, 4, 5, 6, 20, 8, 9, 10, 0, 12, 13, 14, 0, 16, 17, 18, 1, 6, 7, 10, 11 };
        int[] tree = { 1, 20, 3, 0, 5,  6 ,7, 22,9,10,11,24,  13, 14, 15, 0,  17, 18, 19, 0, 2,0,0,0,0 };

        return tree;
    }

    float[] MakeKa(int boneNum)
    {
        List<float> kaList = new List<float>();
        kaList.Add(1);
        for (int i = 1; i < boneNum; i++)
        {
            float ka = Random.Range(0f, 2f);
            kaList.Add(ka);
        }
        return kaList.ToArray(); 
    }

    float KbRange(bool IsRandom)
    {
        
        float Range;
        if (IsRandom)
        {
            Range = Random.Range(0.5f, 2f);
        }
        else
        {
            if (this.Kb[0] == 1f)
                Range = 0.5f;
            else
                Range = 1f;

        }
        Debug.Log("change" + Range.ToString());
        return Range;
        

    }
    void ChangeKa(Human targe,bool IsAlltime, bool IsRandom)
    {
        float vec =  this.PositionofRobot[2].x - targe.bones[0].position.x ;
        
        if (vec < (this.TargetInterval + 0.2  ) && (vec > this.TargetInterval - 0.2) )
        {
            if (!IsAlltime)
            {
                float weTime = 0;
                if (this.InterValKeepedTime == 0) this.InterValKeepedTime = Time.time;
                else { weTime = (Time.time - this.InterValKeepedTime); }
                if (weTime > this.KeepTime)
                {
                    this.Kb[0] = this.KbRange(IsRandom);
                    this.Kb[1] = Kb[0];
                    this.TargetInterval = Mathf.Sqrt((Kb[0] + Kb[1]) / (Ka[0] + Ka[1]));
                    //this.KeepTime = Random.Range(0f, 5f);
                    Debug.Log("change" + this.TargetInterval.ToString() +":"+ this.KeepTime.ToString());
                    this.InterValKeepedTime = 0;
                }
                
            }
            else
            {
                this.Kb[0] = this.KbRange(IsRandom);
                this.Kb[1] = Kb[0];
                this.TargetInterval = Mathf.Sqrt((Kb[0] + Kb[1]) / (Ka[0] + Ka[1]));
                Debug.Log("change" + this.TargetInterval.ToString());
                
            }
            //Debug.Log(this.TargetInterval.ToString());
                
            
            
         }
        
    }
    void ChangeKa(bool IsRandam)
    {
        this.Kb[0] = this.KbRange(IsRandam);
        this.Kb[1] = Kb[0];
        this.TargetInterval = Mathf.Sqrt((Kb[0] + Kb[1]) / (Ka[0] + Ka[1]));

        Debug.Log(this.TargetInterval.ToString());
    }

    //外部から変化
    #region
    public void Change_actModePos(bool isBool)
    {
        if (isBool)
        {
            this.actMode = _actMode.pos;
        }
        else
        {
            this.actMode = _actMode.bone;
        }
    }
    public void Change_demModeThree(bool isBool)
    {
        if (isBool)
        {
            this.demMode = _demMode.threed;
        }
        else
        {
            this.demMode = _demMode.twod; ;
        }
    }
    public void SetIsChangeInterval(bool Is)
    {
        this.IsChangeInterval = Is;
    }
    public void SetIsRange(bool Is)
    {
        this.IsRadam = Is;
    }
    public void SetTimeingAlltime(int i)
    {
        switch (i)
        {
            case 0: this.timeing = _timing.alltime; break;
            case 1: this.timeing = _timing.moment; break;
            case 2: this.timeing = _timing.stay; break;

        }
        
    }
  
    #endregion

    public void IsEnableChange(bool IsE)
    {
        if (this.model != null)
        this.model.SetActiveRecursively(IsE);
    }
}


