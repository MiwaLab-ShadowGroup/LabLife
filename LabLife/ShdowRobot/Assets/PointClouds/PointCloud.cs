﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointCloud : MonoBehaviour
{
    #region
    public GameObject cube;
    private List<GameObject> ListCube = new List<GameObject>();
    public Windows.Kinect.KinectSensor sensor;
    private ushort[] RawData;
    private Windows.Kinect.CameraSpacePoint[] cameraSpacePoints;
    public Windows.Kinect.DepthFrameReader depthreader { get; set; }
    private int imageWidth;
    private int imageHeight;

    public GameObject kinect;

    public enum _Mode {Random, Boder, Contour, ContourBoder,}
    public _Mode mode;

    //パラメータ
    public int maxCubeNum = 5000;
    public Vector2 rangex;
    public Vector2 rangez;
    public float roophight;
    public int range;

    //監視
    public int particulsnum;
    [HideInInspector]
    public Vector3 centerPos;

    #endregion
    // Use this for initialization
    void Start()
    {
        this.sensor = Windows.Kinect.KinectSensor.GetDefault();
        this.depthreader = this.sensor.DepthFrameSource.OpenReader();
        this.imageWidth = 512;
        this.imageHeight = 424;
        this.depthreader.FrameArrived += depthreader_FrameArrived;
        this.RawData = new ushort[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];

        this.ListCube = new List<GameObject>();

        this.cameraSpacePoints = new Windows.Kinect.CameraSpacePoint[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];
        this.sensor.Open();

        this.centerPos = new Vector3();        
    }

    void depthreader_FrameArrived(object sender, Windows.Kinect.DepthFrameArrivedEventArgs e)
    {
        using (var frame = e.FrameReference.AcquireFrame())
        {
            frame.CopyFrameDataToArray(this.RawData);
            sensor.CoordinateMapper.MapDepthFrameToCameraSpace(this.RawData, this.cameraSpacePoints);
            
        }
    }

    

    // Update is called once per frame
    void Update()
    {        

        int cubeCount = 0;

        //this.centerPos = Vector3.zero;
        switch (this.mode)
        {
            case _Mode.Random: this.Contemporary(ref cubeCount); break;
            case _Mode.Boder:  this.Border(ref cubeCount); break;
            case _Mode.Contour: this.Contour(ref cubeCount); break;
            case _Mode.ContourBoder: this.ContourBoder(ref cubeCount); break;
        }
        //this.Contour(ref cubeCount);

        ////重心位置算出
        //this.centerPos /= cubeCount;
        //this.centerPos += this.kinect.transform.position;


        //消す処理
        if (this.ListCube.Count > (cubeCount + 100) )
        {
            for (int i = cubeCount; i < this.ListCube.Count; i++)
            {
                Destroy(this.ListCube[cubeCount]);
                this.ListCube.RemoveAt(cubeCount);
            }

        }

        this.particulsnum = this.ListCube.Count;
      
    }        

   
    void CubeControll(int cubeCount,Vector3 point)
    {
        if (cubeCount < this.ListCube.Count)
        {
            this.ListCube[cubeCount].transform.localPosition = point;
        }
        else
        {
            this.ListCube.Add(Instantiate(this.cube));
            this.ListCube[this.ListCube.Count - 1].transform.parent = this.kinect.transform;
            this.ListCube[this.ListCube.Count - 1].transform.localPosition = point;
        }
    }

    void Contemporary(ref int cubeCount)
    {
        int pointCount = 0;
        for (int i = 0; i < cameraSpacePoints.Length; i++)
        {
            //奥の壁排除
            if (this.cameraSpacePoints[i].Z > this.rangez.x && this.cameraSpacePoints[i].Z < this.rangez.y && this.ListCube.Count < this.maxCubeNum)
            {
                //三次元位置に変更
                Vector3 point = new Vector3(-this.cameraSpacePoints[i].X, this.cameraSpacePoints[i].Y, this.cameraSpacePoints[i].Z);

                //床排除と左右の壁排除                       
                if (point.y < this.roophight && point.y > -this.kinect.transform.position.y && point.x > this.rangex.x && point.x < this.rangex.y && pointCount % this.range == 0)
                {
                    //条件クリアした粒子
                    this.CubeControll(cubeCount, point);

                    cubeCount++;
                }
                pointCount++;
            }

        }
    }
    void Border(ref int cubeCount)
    {

        for (int i = 0; i < this.cameraSpacePoints.Length; i+=this.range * this.imageWidth)
        {
            for(int j =0; j < this.imageWidth; j++)
            {
                //奥の壁排除
                if (this.cameraSpacePoints[i + j].Z > this.rangez.x && this.cameraSpacePoints[i + j].Z < this.rangez.y && this.ListCube.Count < this.maxCubeNum)
                {
                    //三次元位置に変更
                    Vector3 point = new Vector3(-this.cameraSpacePoints[i + j].X, this.cameraSpacePoints[i + j].Y, this.cameraSpacePoints[i + j].Z);

                    //床排除と左右の壁排除                       
                    if (point.y < this.roophight && point.y > -this.kinect.transform.position.y && point.x > this.rangex.x && point.x < this.rangex.y)
                    {
                        //条件クリアした粒子
                        this.CubeControll(cubeCount, point);

                        cubeCount++;
                    }

                }
            }
        }
    }
    void Contour(ref int cubeCount)
    {
        int pointCount = 0;
        for (int i = this.imageWidth; i < cameraSpacePoints.Length- this.imageWidth; i++)
        {
            //奥の壁排除
            if (this.cameraSpacePoints[i].Z > this.rangez.x && this.cameraSpacePoints[i].Z < this.rangez.y && this.ListCube.Count < this.maxCubeNum)
            {
                //三次元位置に変更
                Vector3 point = new Vector3(-this.cameraSpacePoints[i].X, this.cameraSpacePoints[i].Y, this.cameraSpacePoints[i].Z);
                //床排除と左右の壁排除                       
                if (point.y < this.roophight && point.y > -this.kinect.transform.position.y && point.x > this.rangex.x && point.x < this.rangex.y)
                {
                    //条件クリアした粒子
                    //輪郭
                    if (Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i + this.imageWidth].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i - this.imageWidth].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i + 1].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i - 1].Z) > 0.5)
                    {
                        this.CubeControll(cubeCount, point);
                        cubeCount++;

                    }
                    else if(pointCount % this.range == 0)
                    {
                        this.CubeControll(cubeCount, point);
                        cubeCount++;
                    }
                    pointCount++;

                }
            }

        }
    }
    void ContourBoder(ref int cubeCount)
    {
        int pointCount = 0;
        for (int i = this.imageWidth; i < cameraSpacePoints.Length - this.imageWidth; i++)
        {
            //奥の壁排除
            if (this.cameraSpacePoints[i].Z > this.rangez.x && this.cameraSpacePoints[i].Z < this.rangez.y && this.ListCube.Count < this.maxCubeNum)
            {
                //三次元位置に変更
                Vector3 point = new Vector3(-this.cameraSpacePoints[i].X, this.cameraSpacePoints[i].Y, this.cameraSpacePoints[i].Z);
                //床排除と左右の壁排除                       
                if (point.y < this.roophight && point.y > -this.kinect.transform.position.y && point.x > this.rangex.x && point.x < this.rangex.y)
                {
                    //条件クリアした粒子
                    //輪郭
                    if (Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i + this.imageWidth].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i - this.imageWidth].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i + 1].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i - 1].Z) > 0.5)
                    {
                        this.CubeControll(cubeCount, point);
                        cubeCount++;
                    }
                    //輪郭の内側
                    //ボーダー状にする
                    else if((int)(i / this.imageWidth) % this.range == 0)
                    {
                        
                        this.CubeControll(cubeCount, point);
                        cubeCount++;
                    }
                    pointCount++;

                }
            }

        }
    }
}
