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
    public GameObject depth;
    //Local
    CIPCReceiver cipcKinect;
    CIPCReceiverLaserScaner cipcLS;
    PointCloud pointCloud;
    //モード
    public enum _Mode { KinectStation, LaserScaner, Depth, };
    public _Mode DataMode;
    
    //CIPC
    Vector3 centerPos;
    Vector3 preCenterPos;
    List<Vector3> list_humanpos;
    List<Human> List_Human;
    //計算クラス
    CalculatePosition cp;
    #endregion

   
    
    // Use this for initialization
    public int velparameter;
    public Vector4 fieldSize;

    void Start()
    {
        this.cipcKinect = this.CIPCforKinect.GetComponent<CIPCReceiver>();
        this.cipcLS = this.CIPCforLaserScaner.GetComponent<CIPCReceiverLaserScaner>();
        this.pointCloud = this.depth.GetComponent<PointCloud>();

        this.List_Human = new List<Human>();
        this.list_humanpos = new List<Vector3>();

        this.cp = new CalculatePosition(0);
        this.centerPos = new Vector3();
        this.preCenterPos = new Vector3();
       
        Debug.Log("Light Robot");       
    }

    // Update is called once per frame
    void Update()
    {
        //データもとによってそれぞれ計算
        switch (this.DataMode)
        {
            case _Mode.KinectStation: this.MoveByKinectHuman(this.robot); break;
            case _Mode.LaserScaner: this.MoveByPosition(); break;
            case _Mode.Depth: this.MoveByDepth(); break;
        }

    }

    //データ取得
    void Bone()
    {
        this.List_Human = this.cipcKinect.List_Humans;
        if (this.List_Human.Count != 0)
        {
            //位置
            this.centerPos = this.cp.CenterPosition(this.List_Human);
            
        }
    }
    void LaserRangeFinder()
    {
        this.centerPos = this.cp.CenterPosition(this.cipcLS.list_humanpos);
    }
    void Depth()
    {
        try
        {
            this.centerPos = this.pointCloud.centerPos;
            
        }
        catch { }
    }
    
    //動き
    void TwoDMoveCenter()
    {
        //位置
        Vector3 vec = - this.centerPos - this.robot.transform.position;
        vec = new Vector3(vec.x, 0, vec.z);
        vec /= vec.magnitude;
        robot.transform.position += vec / this.velparameter;
    }
    void ThreeDMoveCeneter()
    {
        //位置
        Vector3 vec = - this.centerPos - this.robot.transform.position;
        vec /= vec.magnitude;
        robot.transform.position += vec / this.velparameter;
    }
    void HeightSine()
    {
        //位置
        Vector3 vec = - this.centerPos - this.robot.transform.position;
        //Sinで高さ決定
        vec.y = 0.5f * Mathf.Sin(Time.deltaTime);
        vec /= vec.magnitude;
        robot.transform.position += vec / this.velparameter;
    }
    void HeightPrePos()
    {
        this.centerPos.y += this.cp.CulculateVelocity(this.centerPos, this.preCenterPos).y;
        Vector3 vec = - this.centerPos - this.robot.transform.position;
        vec /= vec.magnitude;
        robot.transform.position += vec / this.velparameter;


        this.preCenterPos = this.centerPos;
    }


    void MoveByKinectHuman(GameObject robot)
    {
        this.List_Human = this.cipcKinect.List_Humans;
        if (this.List_Human.Count != 0)
        {
            
        }
    }
    void MoveByPosition()
    {
        this.list_humanpos = this.cipcLS.list_humanpos;
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
    void MoveByDepth()
    {
        try
        {
            Vector3 center = -this.pointCloud.centerPos;
            if (center.magnitude > 0)
            {
                Vector3 vec = center - this.robot.transform.position;
                vec = new Vector3(vec.x, 0, vec.z);
                vec /= vec.magnitude;
                this.robot.transform.position += vec / this.velparameter;
            }
        }
        catch { }
 

    }
    

}
