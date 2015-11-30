using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CircleLine : MonoBehaviour
{
    public enum _mode { range, CenterPos, SD,RBM,}
    public _mode mode;
    //必要データ
    public GameObject robot;
    public GameObject CIPCforKinect;
    public GameObject CIPCforLS;
    public int Vel;
    public bool IsUseKinet;


    public int range;
    public int radian;

    //
    CalculatePosition CP;
    RandomBoxMuller RBM;
    List<Vector3> list_pos;
    int targetHuman0;
    int targetHuman1;
    float radius;
    Vector3 center;
    Vector3 robotPos;
    Vector3 target;
    Vector3 vec;
    //
    Vector3 preCenter;
    //標準偏差
    double preSD; 

	// Use this for initialization
	void Start ()
    {
        this.CP = new CalculatePosition();
        this.RBM = new RandomBoxMuller();
        this.targetHuman0 = 0;
        this.targetHuman1 = 0;
        this.IsUseKinet = false;

        this.preCenter = Vector3.zero;
        this.preSD = 0;
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (this.IsUseKinet)
        {
            List<Human> list = this.CIPCforKinect.GetComponent<CIPCReceiver>().List_Humans;
            List<Vector3> listPos = new List<Vector3>();
            for(int i= 0; i< list.Count; i++)
            {
                listPos.Add(list[i].bones[0].position);
            }
            this.list_pos = listPos;
        } 
        else
        {
            this.list_pos = this.CIPCforLS.GetComponent<CIPCReceiverLaserScaner>().list_humanpos;
                 
        }
        switch (this.mode)
        {
            case _mode.range:     this.Move();           break;
            case _mode.CenterPos: this.NoisePreCenter(); break;
            case _mode.SD:        this.NoiseSD();        break;
            case _mode.RBM:       this.NoiseBoxMuller(); break;
        }
        


    }

    //円周上を動く
    void Move()
    {
        
        if (this.list_pos.Count != 0)
        {
            this.radius = this.CP.MaxLength(this.list_pos, ref this.targetHuman0, ref targetHuman1) / 2;
            this.center = ( this.list_pos[this.targetHuman0] + this.list_pos[this.targetHuman1] ) / 2 ;
            this.robotPos = this.robot.transform.position;
            this.robotPos.y = 0;

            //円中心からの方向ベクトル
            this.target = (this.robotPos - this.center) / (this.robotPos - this.center).magnitude * ( this.radius + this.range);
            //目標地点をthis.radian分回転して目標点を決める
            this.target = Quaternion.AngleAxis(this.radian, Vector3.up) * this.target + this.center;

            
            //移動
            this.vec = this.target - this.robotPos;
            this.vec /= this.vec.magnitude;
            this.robot.transform.position += this.vec/ this.Vel;
            
        }
        
    }

    //重心の動き分を足す(前後左右問わず) && 回転なし
    void NoisePreCenter()
    {
        
        if (this.list_pos.Count != 0)
        {
            this.radius = this.CP.MaxLength(this.list_pos, ref this.targetHuman0, ref targetHuman1) / 2;
            this.center = (this.list_pos[this.targetHuman0] + this.list_pos[this.targetHuman1]) / 2;
            this.robotPos = this.robot.transform.position;
            this.robotPos.y = 0;

            //目標地点の絶対座標を求める
            this.target = (this.robotPos - this.center) / (this.robotPos - this.center).magnitude * this.radius + this.center;
            //ノイズ
            this.target += (this.center - this.preCenter);

            //移動
            this.vec = this.target - this.robotPos;
            this.vec /= this.vec.magnitude;
            this.robot.transform.position += this.vec / this.Vel;

            this.preCenter = this.center;
        }

    }

    //標準偏差で中心方向にぶれる
    void NoiseSD()
    {
        if (this.list_pos.Count != 0)
        {
            double SD = this.CP.StandardDeviation(this.list_pos);

            this.radius = this.CP.MaxLength(this.list_pos, ref this.targetHuman0, ref targetHuman1) / 2;
            this.center = (this.list_pos[this.targetHuman0] + this.list_pos[this.targetHuman1]) / 2;
            this.robotPos = this.robot.transform.position ;
            this.robotPos.y = 0;

            //目標地点の絶対座標を求める
            this.target = (this.robotPos - this.center) / (this.robotPos - this.center).magnitude * this.radius + this.center;
            //ノイズ 標準偏差の違い分だけ求心方向に動く
            this.target += (float)(this.preSD - SD) * (this.target - this.center);
                 
            //移動
            this.vec = this.target - this.robotPos;
            this.vec /= this.vec.magnitude;
            this.robot.transform.position += this.vec / this.Vel;

            this.preSD = SD;

        }
    }
    
    //ホワイトノイズで前後にぶれる
    void NoiseBoxMuller()
    {
        if (this.list_pos.Count != 0)
        {

            this.radius = this.CP.MaxLength(this.list_pos, ref this.targetHuman0, ref targetHuman1) / 2;
            this.center = (this.list_pos[this.targetHuman0] + this.list_pos[this.targetHuman1]) / 2;
            this.robotPos = this.robot.transform.position;
            this.robotPos.y = 0;

            //目標地点の絶対座標を求める
            this.target = (this.robotPos - this.center) / (this.robotPos - this.center).magnitude * this.radius;
            //中心方向にノイズ
            this.target += (float)this.RBM.next() * (this.target - this.center);

            //移動
            this.vec = this.target - this.robotPos;
            this.vec /= this.vec.magnitude;
            this.robot.transform.position += this.vec / this.Vel;
                       

        }
    }

}
