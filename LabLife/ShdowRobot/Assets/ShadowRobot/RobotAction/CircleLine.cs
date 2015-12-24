using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircleLine : MonoBehaviour
{
    //モード
    public enum _datamode { kinect, LRF, }
    public _datamode dataMode;
    public enum _actionmode { CenterPos, VelOfCenterPos, Random,noiseSD,}
    public _actionmode actionMode;
    public enum _dirctionR {diff, Cross, None,}
    public _dirctionR dirctionR;
    //必要データ
    public GameObject robot;
    public GameObject CIPCforKinect;
    public GameObject CIPCforLS;
    CIPCReceiverLaserScaner cipcForLS;
    CIPCReceiver cipcForKinect;
    //パラメータ
    public float rangeR;
    public float rangeθ;
    //public float rangeVel;
    //
    CalculatePosition CP;
    RandomBoxMuller RBM;
    List<Vector3> list_pos;
    int targetHuman0;
    int targetHuman1;
    float radian;
    float radius;
    Vector3 robotPos;
    Vector3 target;
    int directionOfRotation;
    int intervel;
    //
    Vector3 preCenter;
    Vector3 preHumanCenter;
    Vector3 preCross;
    Vector3 preVel;
    //標準偏差
    double preSD;
    private float preTime;

    // Use this for initialization
    void Start ()
    {
        this.CP = new CalculatePosition();
        this.RBM = new RandomBoxMuller();
        this.targetHuman0 = 0;
        this.targetHuman1 = 0;

        this.cipcForKinect = this.CIPCforKinect.GetComponent<CIPCReceiver>();
        this.cipcForLS = this.CIPCforLS.GetComponent<CIPCReceiverLaserScaner>();

        this.list_pos = new List<Vector3>();

        this.radius = 0;
        
        this.preSD = 0;
        this.target = Vector3.right;
        this.directionOfRotation = 1;
        this.intervel = 5;
        this.preTime = 0;
        this.preCross = Vector3.up;
        this.preHumanCenter = Vector3.zero;
        this.preVel = Vector3.zero;
        this.preCenter = Vector3.zero;

        this.robot.transform.position = new Vector3(0.0f, this.robot.transform.position.y, 0.0f);

        this.Test();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //データ取得
        #region
        //switch (this.dataMode)
        //{
        //    case _datamode.kinect: this.Kinect(); break;
        //    case _datamode.LRF: this.LRF(); break;
        //}

        #endregion
        //動き
        switch (this.actionMode)
        {
            case _actionmode.CenterPos:      this.MoveCircleByPos(); break;
            case _actionmode.VelOfCenterPos: this.MoveCircleByVel(); break;
            case _actionmode.Random:         this.MoveCircleByRandom(); break;
            case _actionmode.noiseSD:        this.NoiseSD(); break;
        }
    }

    //データ取得関数
    #region
    void Kinect()
    {
        List<Vector3> listPos = new List<Vector3>();
        for (int i = 0; i < this.cipcForKinect.List_Humans.Count; i++)
        {
            listPos.Add(this.cipcForKinect.List_Humans[i].bones[0].position);
        }
        this.list_pos = listPos;
    }
    void LRF()
    {
        this.list_pos = this.cipcForLS.list_humanpos;
    }

    void Test()
    {
        this.list_pos.Add(new Vector3(0, 0, 0));
        this.list_pos.Add(new Vector3(1, 0, 0));
    }
    #endregion


    //動き
    #region
    //円周を動く
    void MoveCircleByPos()
    {

       if (this.list_pos.Count > 0)
       {
            //半径
            float R = (this.CP.MaxLength(this.list_pos, ref this.targetHuman0, ref targetHuman1) / 2 + this.rangeR);
            this.radius += ( R - this.radius)  * Time.deltaTime ;
            //回転中心
            Vector3 centerPos= (this.list_pos[this.targetHuman0] + this.list_pos[this.targetHuman1]) / 2;

            //回転角度
            //角速度は人の重心（回転中心からの相対）
            Vector3 humansPos = this.CP.CenterPosition(this.list_pos) ;
            ////回転方向はロボットとの外積で決定
            this.robotPos = this.robot.transform.position;          
            this.robotPos.y = 0.0f;
            float dir = 1;
            switch (this.dirctionR)
            {
                case _dirctionR.Cross:
                    Vector3 cross;
                    if (this.robotPos != Vector3.zero)
                    {
                        cross = Vector3.Cross(humansPos, this.robotPos);
                        cross /= cross.magnitude;
                    }
                    else { cross = this.preCross; }
                    this.preCross = cross;
                    dir = cross.y;
                    break;
                case _dirctionR.diff:
                    if (humansPos.magnitude - this.preHumanCenter.magnitude > 0)
                    {
                        dir = 1;
                    }
                    else { dir = -1; }
                    break;
                case _dirctionR.None:
                    break;
            }

            this.radian = dir * humansPos.magnitude * Time.deltaTime * this.rangeθ;

            //円中心からの方向ベクトル
            this.target = centerPos + (Quaternion.AngleAxis(this.radian, Vector3.up) * ((this.target - this.preCenter) / (this.target - this.preCenter).magnitude * this.radius));
            this.target.y = this.robotPos.y;
            //移動
            
            this.robot.transform.position = this.target;
            this.preCenter = centerPos;
            this.preHumanCenter = humansPos;
        }

    }
    void MoveCircleByVel()
    {

        if (this.list_pos.Count > 0)
        {
            //半径
            float R = (this.CP.MaxLength(this.list_pos, ref this.targetHuman0, ref targetHuman1) / 2 + this.rangeR);
            this.radius += (R - this.radius) * Time.deltaTime;
            //回転中心
            Vector3 centerPos = (this.list_pos[this.targetHuman0] + this.list_pos[this.targetHuman1]) / 2;

            //回転角度
            //角速度は人の重心速度（速度回転中心からの相対）
            Vector3 humansPos = this.CP.CenterPosition(this.list_pos);
            Vector3 centerVel =  (humansPos - this.preHumanCenter) / Time.deltaTime;
            //回転方向はロボットとの外積で決定
            this.robotPos = this.robot.transform.position;
            this.robotPos.y = 0;
            float dir = 1;
            switch (this.dirctionR)
            {
                case _dirctionR.Cross:
                    Vector3 cross;
                    if (this.robotPos != Vector3.zero)
                    {
                        cross = Vector3.Cross(humansPos, this.robotPos);
                        cross /= cross.magnitude;
                    }
                    else { cross = this.preCross; }
                    this.preCross = cross;
                    dir = cross.y;
                    break;
                case _dirctionR.diff:
                    if (centerVel.magnitude - this.preVel.magnitude > 0)
                    {
                        dir = 1;
                    }
                    else { dir = -1; }
                    break;
                case _dirctionR.None:
                    break;
            }
            this.radian = dir * centerVel.magnitude * Time.deltaTime * this.rangeθ;


            //円中心からの方向ベクトル
            this.target = centerPos + (Quaternion.AngleAxis(this.radian, Vector3.up) * ((this.target - this.preCenter) / (this.target - this.preCenter).magnitude * this.radius));
            this.target.y = this.robotPos.y;
            //移動
            this.robot.transform.position = this.target;
            //値の更新
            this.preCenter = centerPos;
            this.preHumanCenter = humansPos;
            this.preVel = centerVel;
        }

    }   
    void MoveCircleByRandom()
    {

        if (this.list_pos.Count > 0)
        {
            //半径
            float R = (this.CP.MaxLength(this.list_pos, ref this.targetHuman0, ref targetHuman1) / 2 + this.rangeR);
            this.radius += (R - this.radius) * Time.deltaTime;
            //回転中心
            Vector3 centerPos = (this.list_pos[this.targetHuman0] + this.list_pos[this.targetHuman1]) / 2;

            //回転角度
            //角速度は人の重心（速度回転中心からの相対）
            Vector3 humansPos = this.CP.CenterPosition(this.list_pos);
            Vector3 centerVel = (humansPos - this.preHumanCenter) / Time.deltaTime;
            //回転方向はthis.interval[s]ごとにランダムに決定
            this.robotPos = this.robot.transform.position;
            float time = Time.realtimeSinceStartup;
            if ((int)time % this.intervel == 0 && (time - this.preTime) > 1f)
            {
                this.preTime = time;

                int pal = Random.Range(256, 0) % 2;
                if (pal == 1)
                {
                    //回転方向反転
                    this.directionOfRotation = 1;
                }
                else
                {
                    this.directionOfRotation = -1;
                }
                Debug.Log(this.directionOfRotation);
            }
            this.radian = this.directionOfRotation * centerVel.magnitude * Time.deltaTime * this.rangeθ;

            //円中心からの方向ベクトル
            this.target = centerPos + (Quaternion.AngleAxis(this.radian, Vector3.up) * ((this.target - this.preCenter) / (this.target - this.preCenter).magnitude * this.radius));
            this.target.y = this.robotPos.y;
            //移動

            this.robot.transform.position = this.target;
            //値の更新
            this.preCenter = centerPos;
            this.preHumanCenter = humansPos;
        }

    }

    //標準偏差で中心方向にぶれる
    void NoiseSD()
    {
        if (this.list_pos.Count != 0)
        {
            //半径
            double SD = this.CP.StandardDeviation(this.list_pos);
            float noise = (float)(SD - this.preSD);
            float R = this.CP.MaxLength(this.list_pos, ref this.targetHuman0, ref targetHuman1) / 2 + noise;
            this.radius += ( R - this.radius)  * Time.deltaTime ;

            //回転中心
            Vector3 centerPos = (this.list_pos[this.targetHuman0] + this.list_pos[this.targetHuman1]) / 2;
            //回転角度
            //角速度は人の重心（速度回転中心からの相対）
            Vector3 humansPos = this.CP.CenterPosition(this.list_pos);
            Vector3 centerVel = (humansPos - this.preHumanCenter) / Time.deltaTime;
            //回転方向はロボットとの外積で決定
            this.robotPos = this.robot.transform.position;
            this.robotPos.y = 0;
            float dir = 1;
            switch (this.dirctionR)
            {
                case _dirctionR.Cross:
                    Vector3 cross;
                    if (this.robotPos != Vector3.zero)
                    {
                        cross = Vector3.Cross(humansPos, this.robotPos);
                        cross /= cross.magnitude;
                    }
                    else { cross = this.preCross; }
                    this.preCross = cross;
                    dir = cross.y;
                    break;
                case _dirctionR.diff:
                    if (centerVel.magnitude - this.preVel.magnitude > 0)
                    {
                        dir = 1;
                    }
                    else { dir = -1; }
                    break;
                case _dirctionR.None:
                    break;
            }
            this.radian = dir * centerVel.magnitude * Time.deltaTime * this.rangeθ;

            //目標地点の絶対座標を求める
            //ノイズ 標準偏差の違い分だけ求心方向に動
            this.target = centerPos + (Quaternion.AngleAxis(this.radian, Vector3.up) * ((this.target - this.preCenter) / (this.target - this.preCenter).magnitude * this.radius));
            this.target.y = this.robotPos.y;

            //移動
            this.robot.transform.position = this.target;
            //値の更新
            this.preCenter = centerPos;
            this.preSD = SD;
            this.preHumanCenter = humansPos;
            this.preVel = centerVel;

        }
    }    

    ////プロトタイプ
    //void Move()
    //{
        
    //    if (this.list_pos.Count != 0)
    //    {
    //        this.radius = this.CP.MaxLength(this.list_pos, ref this.targetHuman0, ref targetHuman1) / 2;
    //        this.center = ( this.list_pos[this.targetHuman0] + this.list_pos[this.targetHuman1] ) / 2 ;
    //        this.robotPos = this.robot.transform.position;
    //        this.robotPos.y = 0;

    //        //円中心からの方向ベクトル
    //        this.target = (this.robotPos - this.center) / (this.robotPos - this.center).magnitude * ( this.radius + this.rangeR);
    //        //目標地点をthis.radian分回転して目標点を決める
    //        this.target = Quaternion.AngleAxis(this.radian, Vector3.up) * this.target + this.center;

            
    //        //移動
    //        this.vec = this.target - this.robotPos;
    //        this.vec /= this.vec.magnitude;
    //        /this.robot.transform.position += this.vec/ this.Vel;
            
    //    }
        
    //}
    ////重心の動き分を足す(前後左右問わず) && 回転なし
    //void NoisePreCenter()
    //{
        
    //    if (this.list_pos.Count != 0)
    //    {
    //        this.radius = this.CP.MaxLength(this.list_pos, ref this.targetHuman0, ref targetHuman1) / 2;
    //        this.center = (this.list_pos[this.targetHuman0] + this.list_pos[this.targetHuman1]) / 2;
    //        this.robotPos = this.robot.transform.position;
    //        this.robotPos.y = 0;

    //        //目標地点の絶対座標を求める
    //        this.target = (this.robotPos - this.center) / (this.robotPos - this.center).magnitude * this.radius + this.center;
    //        //ノイズ
    //        this.target += (this.center - this.preCenter);

    //        //移動
    //        this.vec = this.target - this.robotPos;
    //        this.vec /= this.vec.magnitude;
    //        this.robot.transform.position += this.vec / this.Vel;

    //        this.preCenter = this.center;
    //    }

    //}
    
    ////ホワイトノイズで前後にぶれる
    //void NoiseBoxMuller()
    //{
    //    if (this.list_pos.Count != 0)
    //    {
    //        this.radius = this.CP.MaxLength(this.list_pos, ref this.targetHuman0, ref targetHuman1) / 2;
    //        this.center = (this.list_pos[this.targetHuman0] + this.list_pos[this.targetHuman1]) / 2;
    //        this.robotPos = this.robot.transform.position;
    //        this.robotPos.y = 0;

    //        //目標地点の絶対座標を求める
    //        this.target = (this.robotPos - this.center) / (this.robotPos - this.center).magnitude * this.radius;
    //        //中心方向にノイズ
    //        this.target += (float)this.RBM.next() * (this.target - this.center);

    //        //移動
    //        this.vec = this.target - this.robotPos;
    //        this.vec /= this.vec.magnitude;
    //        this.robot.transform.position += this.vec / this.Vel;
                       

    //    }
    //}
    #endregion
}
