using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Not人型 But 光源ロボット
/// 目標：
///     人同士の関係によって動く
///     人に影響をうけつつ影響を与える
/// </summary>
/// 


public class MoveByCenterPos : MonoBehaviour
{
    #region
    //public GameObject model;
    public GameObject robot;
    public GameObject CIPCforLaserScaner;
    public GameObject CIPCforKinect;
    //モード
    bool IsKinectHuman;
    //CIPC
    List<Vector3> list_humanpos;
    List<Human> List_Human;
    //計算クラス
    CalculatePosition cp;

    #endregion

    //実験用　randonな動き
    Vector3 vec;
    int frame;
    int interval;
    public bool IsRandom;
    // Use this for initialization
    public int velparameter;
    public Vector4 fieldSize;

    void Start()
    {
        this.List_Human = new List<Human>();
        this.list_humanpos = new List<Vector3>();

        this.vec = Vector3.zero;
        this.frame = 0;
        this.interval = 5;

        this.IsKinectHuman = true;

        this.cp = new CalculatePosition(0);

        //this.robotPosition = new RobotPosition();
        //this.lightAction = new LightAction();
        Debug.Log("Light Robot");
    }

    // Update is called once per frame
    void Update()
    {
        if (this.IsRandom) { this.MoveRandom(); }
        else if (this.IsKinectHuman) { this.MoveByKinectHuman(this.robot); }
        else { this.MoveByPosition(); }
    }

    void MoveByKinectHuman(GameObject robot)
    {
        this.List_Human = this.CIPCforKinect.GetComponent<CIPCReceiver>().List_Humans;
        if (this.List_Human.Count != 0)
        {
            //位置
            Vector3 center = this.cp.CenterPosition(this.List_Human);
            Vector3 vec = center - this.robot.transform.position;
            vec = new Vector3(vec.x, 0, vec.z);
            vec /= vec.magnitude;
            robot.transform.position += vec / this.velparameter;
            //this.lightAction.Action(this.List_Human, ref this.robot);
        }
    }
    void MoveByPosition()
    {
        this.list_humanpos = this.CIPCforLaserScaner.GetComponent<CIPCReceiverLaserScaner>().list_humanpos;
        if (this.list_humanpos.Count != 0)
        {
            Vector3 center = -this.cp.CenterPosition(this.list_humanpos);
            Vector3 vec = center - this.robot.transform.position;
            vec = new Vector3(vec.x, 0, vec.z);
            vec /= vec.magnitude;
            this.robot.transform.position += vec / this.velparameter;
            //this.robot.transform.position += new Vector3(vec.x, 0, vec.z);

        }

    }
    void MoveRandom()
    {

        if (this.frame % (this.interval * 60) == 0)
        {
            //位置
            this.vec = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
            this.interval = Random.Range(0, 7);
            this.frame = 1;
        }
        //位置
        this.vec /= (this.vec.magnitude * this.velparameter);
        Vector3 pos = this.robot.transform.position + vec;
        if (pos.x > this.fieldSize.x && pos.x < this.fieldSize.y && pos.z > this.fieldSize.z && pos.z < this.fieldSize.w)
        {
            this.robot.transform.position += vec;
            this.frame++;

        }
        else { this.frame = 0; }
        //this.lightAction.Action(this.List_Human, ref this.robot);

    }

    //パラメータ
    public void ChangeMode(bool Iskinecthuman)
    {
        this.IsKinectHuman = Iskinecthuman;
    }
    public void ChangeVel(bool Is)
    {
        //this.robotPosition.ChangeVel(Is);
    }
    public void ChangeIsHigh(bool Is)
    {
        //this.robotPosition.ChangeIsHigh(Is);
    }
    public void ChangeIsJumpVel(bool Is)
    {
        //this.robotPosition.ChangeIsJumpVel(Is);
    }
    public void ChangeJump(bool Is)
    {
        //this.robotPosition.ChangeJump(Is);
    }

}
