using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Threading;
using System.Collections.Generic;
using System;

public class centerpos : MonoBehaviour
{

    BinaryReader reader;
    BinaryReader reader10frame;
    [HideInInspector]
    public ushort[] readData;
    [HideInInspector]
    public ushort[] read10frameData;
    ushort[] ArchiveData;

    int datalength;
    int datalength10frame;
    public bool OpenFileChoose = false;

    public bool SaveFileChoose = false;

    string FilePath;
    bool IsStart = false;
    Thread thread;
    bool IsSave = false;
    bool Isreader = true;

    BinaryWriter writer;
    string FolderPath;
    public string SaveFilename;

    public bool ReadStart = false;
    public bool ReadStop = false;
    public bool NoUpdate = false;

    public Windows.Kinect.KinectSensor sensor;
    private Windows.Kinect.CameraSpacePoint[] cameraSpacePoints;
    private Windows.Kinect.CameraSpacePoint[] cameraSpacePoints10frame;

    public Windows.Kinect.DepthFrameReader depthreader { get; set; }

    private int imageWidth;
    private int imageHeight;

    public Vector2 rangex;
    public Vector2 rangez;
    public float roophight;

    public GameObject kinect;

    depthinfo depthcamerapoint;

    int count = 0;

    private List<GameObject> ListCube = new List<GameObject>();
    public GameObject cube;
    //public GameObject cube2;

    public int particulsnum;

    Thread rangethread;

    Vector3 hidePositon;

    int framecount = 0;
    bool Is10frame = false;
    bool Is10frameread = false;

    //test
    //public GameObject cylinder;
    double distance;

    
    public Vector3 centerPos;

    StreamWriter streamwriter;

    int framenum = 0;
    // Use this for initialization
    void Start()
    {

        imageWidth = 512;
        imageHeight = 424;

        readData = new ushort[512 * 424];
        //read10frameData = new ushort[512 * 424];
        ArchiveData = new ushort[512 * 424];

        this.sensor = Windows.Kinect.KinectSensor.GetDefault();
        this.depthreader = this.sensor.DepthFrameSource.OpenReader();

        this.cameraSpacePoints = new Windows.Kinect.CameraSpacePoint[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];
        //this.cameraSpacePoints10frame = new Windows.Kinect.CameraSpacePoint[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];
        this.sensor.Open();

        depthcamerapoint = new depthinfo();
        this.depthcamerapoint.DivideCameraSpacePoint = new Windows.Kinect.CameraSpacePoint[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];

        this.hidePositon = new Vector3(0, -100, 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (NoUpdate) return;

        int cubeCount = 0;

        if (OpenFileChoose)
        {
            FilePath = EditorUtility.OpenFilePanel("ファイル選択", "　", "　");

            if (FilePath != null)
            {
                IsStart = true;
                OpenFileChoose = false;
            }
        }

        if (SaveFileChoose)
        {
            FolderPath = EditorUtility.SaveFolderPanel("フォルダ選択", " ", "default ");
            //SaveFileChoose = false;
            if (FolderPath != null)
            {
                //IsSave = true;
                SaveFileChoose = false;
                IsSave = true;
            }
        }

        if (IsStart)
        {
            this.reader = new BinaryReader(File.OpenRead(FilePath));
            IsStart = false;
        }
        if (IsSave)
        {
            this.streamwriter = new StreamWriter(File.OpenWrite(this.FolderPath + @"\" +  this.SaveFilename + ".csv"));
            this.IsSave = false;
        }

        if (ReadStart)
        {
            ReadData();
            
        }


        if (readData != null)
        {
            this.sensor.CoordinateMapper.MapDepthFrameToCameraSpace(this.readData, this.cameraSpacePoints);
        }


        //隠す処理
        if (this.ListCube.Count > cubeCount)
        {
            for (int i = cubeCount; i < this.ListCube.Count; i++)
            {
                this.ListCube[i].transform.position = this.hidePositon;
            }
        }

        this.particulsnum = this.ListCube.Count;

        ExpressHuman(ref cubeCount);

        //CenterPos();
        CenterPos();
        
        if (ReadStart)
        {
            framenum++;
            if (streamwriter != null)
            {
                streamwriter.Write(framenum);
                streamwriter.Write(",");
                streamwriter.Write(centerPos.x);
                streamwriter.Write(",");
                streamwriter.Write(centerPos.z);
                streamwriter.Write(",");
                streamwriter.Write(centerPos.y);
                streamwriter.Write("\n");
            }
            else
            {
                Debug.Log("null");
            }
        }

    }

    void ReadData()
    {
        if (ReadStart)
        {
            this.datalength = this.reader.ReadInt32();

            for (int i = 0; i < datalength; i++)
            {
                this.readData[i] = this.reader.ReadUInt16();
            }

            if (reader.PeekChar() == -1)
            {
                //Debug.Log("end");
                reader.Close();
                ReadStart = false;
                if(streamwriter != null)
                {
                    streamwriter.Close();
                }
            }

            if (ReadStop)
            {
                reader.Close();
                ReadStop = false;
                ReadStart = false;
                if (streamwriter != null)
                {
                    streamwriter.Close();
                }
            }
            //Debug.Log("OK");
        }
        else
        {
            if (reader != null)
            {
                reader.Close();
            }
        }


    }

    void Read10frameData()
    {
        if (Is10frameread)
        {
            //Debug.Log("ok2");
            this.datalength10frame = this.reader10frame.ReadInt32();

            for (int i = 0; i < datalength10frame; i++)
            {
                this.read10frameData[i] = this.reader10frame.ReadUInt16();
            }

            if (reader10frame.PeekChar() == -1)
            {
                reader10frame.Close();
                Is10frameread = false;
            }

            if (ReadStop)
            {
                ReadStop = false;
                Is10frameread = false;
            }
        }
        else
        {
            if (reader10frame != null)
            {
                reader10frame.Close();
            }
        }
    }

    void OnDestroy()
    {
        if (FilePath != null)
        {
            FilePath = null;
        }
        if (reader != null)
        {
            reader.Close();
        }
        if (reader10frame != null)
        {
            reader10frame.Close();
        }
        if (writer != null)
        {
            writer.Close();
        }
        if (FolderPath != null)
        {
            FolderPath = null;
        }
        if (streamwriter != null)
        {
            streamwriter.Close();
        }
    }

    void CubeControll(int cubeCount, Vector3 point)
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

    //void CubeControll2(int cubeCount, Vector3 point)
    //{
    //    if (cubeCount < this.ListCube.Count)
    //    {
    //        this.ListCube[cubeCount].transform.localPosition = point;
    //    }
    //    else
    //    {
    //        this.ListCube.Add(Instantiate(this.cube2));
    //        this.ListCube[this.ListCube.Count - 1].transform.parent = this.kinect.transform;
    //        this.ListCube[this.ListCube.Count - 1].transform.localPosition = point;
    //    }
    //}

    //void RangeChoose()
    //{
    //    if (Is10frameread)
    //    {
    //        #region
    //        ////脚
    //        //List<int> List_rangepoint = new List<int>();
    //        ////人の位置
    //        //List<double> List_LegBoder = new List<double>();
    //        //List<Vector3> List_humanpoint = new List<Vector3>();

    //        ////足の検出
    //        //for (int i = 0; i < this.cameraSpacePoints.Length; i++)
    //        //{
    //        //    if (this.cameraSpacePoints[i].Z > this.rangez.x && this.cameraSpacePoints[i].Z < this.rangez.y
    //        //        && this.cameraSpacePoints[i].Y < this.roophight && this.cameraSpacePoints[i].Y > -this.kinect.transform.position.y
    //        //        && this.cameraSpacePoints[i].X > this.rangex.x && this.cameraSpacePoints[i].X < this.rangex.y)
    //        //    {
    //        //        if (this.cameraSpacePoints[i].Y < 0.2 && this.cameraSpacePoints[i].Y > -0.19)
    //        //        {
    //        //            List_rangepoint.Add(i);
    //        //        }
    //        //    }
    //        //}

    //        //for (int i = 0; i < List_rangepoint.Count - 1; i++)
    //        //{
    //        //    Windows.Kinect.CameraSpacePoint pt0 = this.cameraSpacePoints[List_rangepoint[i]];
    //        //    Windows.Kinect.CameraSpacePoint pt1 = this.cameraSpacePoints[List_rangepoint[i + 1]];
    //        //    float lengthZ = Math.Abs(pt0.Z - pt1.Z);
    //        //    float lengthX = Math.Abs(pt0.X - pt1.X);
    //        //    if ((lengthZ > 0.2 || lengthX > 0.2))
    //        //    {
    //        //        List_LegBoder.Add(List_rangepoint[i]);
    //        //    }
    //        //}
    //        ////Debug.Log(List_LegBoder.Count);

    //        ////足の位置算出
    //        //for (int i = 0; i < List_LegBoder.Count / 2; i += +2)
    //        //{
    //        //    Windows.Kinect.CameraSpacePoint pt0 = this.cameraSpacePoints[List_rangepoint[i]];
    //        //    Windows.Kinect.CameraSpacePoint pt1 = this.cameraSpacePoints[List_rangepoint[i + 1]];
    //        //    Vector3 point = new Vector3((pt0.X + pt1.X) / 2, (pt0.Y + pt1.Y) / 2, (pt0.Z + pt1.Z) / 2);
    //        //    List_humanpoint.Add(point);
    //        //}

    //        //Debug.Log(cylinder.transform.localPosition.x + ", " + cylinder.transform.localPosition.y + ", " + cylinder.transform.localPosition.z + ", ");

    //        ////Debug.Log(List_humanpoint.Count);
    //        ////点群を動かす
    //        //if (List_humanpoint.Count > 0)
    //        //{
    //        //    //this.cylinder.transform.localPosition = new Vector3( - List_humanpoint[0].x, List_humanpoint[0].y, List_humanpoint[0].z);
    //        //    for (int i = 0; i < this.cameraSpacePoints.Length; i++)
    //        //    {
    //        //        //humanpoint[0]からの距離が範囲内か判定
    //        //        if (Math.Abs(this.cameraSpacePoints[i].X - List_humanpoint[0].x) < 0.5
    //        //            && Math.Abs(this.cameraSpacePoints[i].Z - List_humanpoint[0].z) < 0.5
    //        //            && this.cameraSpacePoints[i].Y < this.roophight && this.cameraSpacePoints[i].Y > -this.kinect.transform.position.y)
    //        //        {
    //        //            //条件クリア
    //        //            Vector3 point = new Vector3(-this.cameraSpacePoints[i].X, this.cameraSpacePoints[i].Y, this.cameraSpacePoints[i].Z);

    //        //            this.CubeControll(cubeCount, point);
    //        //            cubeCount++;
    //        //        }
    //        //    }
    //        //}
    //        #endregion

    //        writer.Write(ArchiveData.Length);

    //        for (int i = 0; i < cameraSpacePoints10frame.Length; i++)
    //        {
    //            this.distance = Math.Sqrt(Math.Pow(cameraSpacePoints10frame[i].X - (-cylinder.transform.localPosition.x), 2) + Math.Pow(cameraSpacePoints10frame[i].Z - cylinder.transform.localPosition.z, 2));

    //            if (this.distance < cylinder.transform.localScale.x)
    //            {
    //                this.ArchiveData[i] = this.read10frameData[i];
    //            }
    //            else
    //            {
    //                this.ArchiveData[i] = 0;
    //            }
    //            writer.Write(ArchiveData[i]);

    //        }
    //    }
    //}

    void ExpressHuman(ref int cubeCount)
    {

        for (int i = this.imageWidth; i < cameraSpacePoints.Length - this.imageWidth; i++)
        {
            //奥の壁排除
            if (this.cameraSpacePoints[i].Z > this.rangez.x && this.cameraSpacePoints[i].Z < this.rangez.y)
            {
                //三次元位置に変更
                Vector3 point = new Vector3(-this.cameraSpacePoints[i].X, this.cameraSpacePoints[i].Y, this.cameraSpacePoints[i].Z);

                //床排除と左右の壁排除                       
                if (point.y < this.roophight && point.y > - this.kinect.transform.position.y && point.x > this.rangex.x && point.x < this.rangex.y)
                {

                    if (i % 20 == 0)
                    {
                        this.CubeControll(cubeCount, point);
                        cubeCount++;


                    }
                    
                }
            }
        }
    }

    //void ExpressHuman2(ref int cubeCount)
    //{
    //    for (int i = this.imageWidth; i < cameraSpacePoints10frame.Length - this.imageWidth; i++)
    //    {
    //        //奥の壁排除
    //        if (this.cameraSpacePoints10frame[i].Z > this.rangez.x && this.cameraSpacePoints10frame[i].Z < this.rangez.y)
    //        {
    //            //三次元位置に変更
    //            Vector3 point10frame = new Vector3(-this.cameraSpacePoints10frame[i].X, this.cameraSpacePoints10frame[i].Y, this.cameraSpacePoints10frame[i].Z);

    //            //床排除と左右の壁排除                       
    //            if (point10frame.y < this.roophight && point10frame.y > -this.kinect.transform.position.y && point10frame.x > this.rangex.x && point10frame.x < this.rangex.y)
    //            {
    //                if (i % 20 == 0)
    //                {
    //                    this.CubeControll2(cubeCount, point10frame);
    //                    cubeCount++;
    //                }
    //            }
    //        }
    //    }
    //}

    void CenterPos()
    {
        this.centerPos = Vector3.zero;
        for (int i = 0; i < this.ListCube.Count; i++)
        {
            if(this.ListCube[i].transform.position.y > 0)
            {
                this.centerPos += this.ListCube[i].transform.position;

            }
        }
        this.centerPos /= this.ListCube.Count;
    }

}
