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
    GameObject robotLight;

    public GameObject CIPCforLaserScaner;
    public GameObject CIPCforKinect;
    public GameObject depth;
    //Local
    CIPCReceiver cipcKinect;
    CIPCReceiverLaserScaner cipcLS;
    PointCloud pointCloud;
    //モード
    public enum _DataMode { KinectStation, LaserScaner, Depth, };
    public _DataMode DataMode;
    public enum _Mode { field, hight, randomhight, sinhight,}
    public _Mode Mode;
    public bool IsLight;
    //CIPC
    Vector3 centerPos;
    Vector3 preCenterPos;
    List<Vector3> list_humanpos;
    List<Human> List_Human;
    //計算クラス
    CalculatePosition cp;
    float pretime;
    float randomy;
    #endregion

    // Use this for initialization
    public int velparameter;
    public float angularVelocity;

    void Start()
    {
        this.robotLight = this.robot.transform.FindChild("RobotLight").gameObject;

        this.cipcKinect = this.CIPCforKinect.GetComponent<CIPCReceiver>();
        this.cipcLS = this.CIPCforLaserScaner.GetComponent<CIPCReceiverLaserScaner>();
        this.pointCloud = this.depth.GetComponent<PointCloud>();
        
        this.List_Human = new List<Human>();
        this.list_humanpos = new List<Vector3>();

        this.cp = new CalculatePosition(0);
        this.centerPos = Vector3.zero;
        this.preCenterPos = new Vector3();

        this.pretime = 0;

        Debug.Log("Light Robot");       
    }

    // Update is called once per frame
    void Update()
    {
        //データもとによってそれぞれ計算
        switch (this.DataMode)
        {
            case _DataMode.KinectStation: this.Bone(); break;
            case _DataMode.LaserScaner: this.LaserRangeFinder(); break;
            case _DataMode.Depth: this.Depth(); break;
        }

        if(this.centerPos != Vector3.zero)
        {
            switch (this.Mode)
            {
                case _Mode.field: this.Field(); break;
                case _Mode.hight: this.CenterHight(); break;
                case _Mode.randomhight: this.RandomHight(); break;
                case _Mode.sinhight: this.SinHight(); break;
            }
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
            //this.centerPos.x = -this.centerPos.x;
            
        }
        catch { }
    }
    
    //動き
    void Field()
    {
        //位置
        Vector3 vec = - this.centerPos - this.robot.transform.position;
        vec.y = 0;
        vec /= vec.magnitude;
        this.robot.transform.position += vec / this.velparameter;

    }
    void CenterHight()
    {
        //位置
        this.centerPos.x *= -1;
        this.centerPos.z *= -1;
        Vector3 vec = this.centerPos - this.robot.transform.position;
        if (this.IsLight)
        {
            Vector3 lightVec = this.centerPos - this.robotLight.transform.position;
            lightVec.x = 0;
            lightVec.z = 0;
            this.robotLight.transform.position += lightVec;
            vec.y = 0;
        }
        vec /= vec.magnitude;
        robot.transform.position += vec / this.velparameter;
    }
    void SinHight()
    {
        
        //Sinで高さ決定
        float y = 1.5f + Mathf.Sin(this.angularVelocity * Time.fixedTime);

        if (this.IsLight)
        {
            Vector3 lightVec = new Vector3(0, 1f + y - this.robotLight.transform.position.y, 0) ;
            
            this.robotLight.transform.position += lightVec;
            //Debug.Log(lightVec);
            this.centerPos.y = 0;
        }
        else this.centerPos.y = - y ;
        //位置
        Vector3 vec = -this.centerPos - this.robot.transform.position;
        vec /= vec.magnitude;
        this.robot.transform.position += vec / this.velparameter;
    }

    void RandomHight()
    {
        
        //位置
        //ランダムで高さ決定
        
        float time = Time.fixedTime;
        if ((int)time % 5 == 0 && ((time - this.pretime) > 1f || this.pretime == 0))
        {
            this.randomy = Random.Range(-50, 50);
            this.pretime = time;
            //Debug.Log("change +" + this.centerPos);

        }

        this.centerPos.y = this.randomy;
        Vector3 vec = - this.centerPos - this.robot.transform.position;

        if (this.IsLight)
        {
            Vector3 lightVec = new Vector3(0, this.randomy - this.robotLight.transform.position.y, 0);
            lightVec /= lightVec.magnitude;
            float y = this.robotLight.transform.position.y + lightVec.y / this.velparameter;
            if ( y > 0.5f && y < 2.5f ) this.robotLight.transform.position += lightVec / this.velparameter;
            else this.randomy = Random.Range(-50, 50);
            vec.y = 0;
        }

        vec /= vec.magnitude;
        //Debug.Log(vec);
        this.robot.transform.position += vec / this.velparameter;
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
