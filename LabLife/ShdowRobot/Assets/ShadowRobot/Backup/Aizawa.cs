using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class Aizawa
{
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

    Human robotBone;
    float KeepTime;
    float InterValKeepedTime;
    float TargetInterval;
    #endregion
    public Aizawa()
    {
        this.robotBone = new Human();
        this.Init();
    }

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
            this.PositionofRobot[i] = new Vector3(10, 0, 0) - this.Vec * this.initLength * i * Time.deltaTime;
            this.PositionofHuman[i] = new Vector3(0, 0, 0) + this.Vec * this.initLength * i * Time.deltaTime;
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


    float KbRange(bool IsRandom)
    {
        float Range;
        if (IsRandom)
        {
            Range = UnityEngine.Random.Range(0.5f, 2f);
        }
        else
        {
            Range = 2 * Mathf.Sin(Time.time);

        }
        //Debug.Log("change");
        return Range;
    }
    void ChangeKa(Human targe, bool IsAlltime, bool IsRandom)
    {
        float vec = this.PositionofRobot[2].x - targe.bones[0].position.x;

        if (vec < (this.TargetInterval + 0.5) && (vec > this.TargetInterval - 0.5))
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
                    Debug.Log("change" + this.TargetInterval.ToString() + ":" + this.KeepTime.ToString());
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
    public void ChangeKb(float kb)
    {
        this.Kb[0] = kb;
        this.Kb[1] = kb;
    }

}

