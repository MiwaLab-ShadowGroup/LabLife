using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointCloud : MonoBehaviour
{
    public GameObject cube;
    private List<GameObject> ListCube = new List<GameObject>();
    public Windows.Kinect.KinectSensor sensor;
    private ushort[] RawData;

    private Windows.Kinect.CameraSpacePoint[] cameraSpacePoints;

    public Windows.Kinect.DepthFrameReader depthreader { get; set; }

    public GameObject kinect;
   
    //パラメータ
    public int maxCubeNum = 5000;
    public Vector2 rangex;
    public Vector2 rangez;
    public float roophight;
    public int range;
    public int particulsnum;

    //Save
    [HideInInspector]
    public ushort[] SaveRawData;
    public bool IsArchive;
    public GameObject ReadDepth;
    ReadDepth saveData;
    
    
    // Use this for initialization
    void Start()
    {
        this.sensor = Windows.Kinect.KinectSensor.GetDefault();
        this.depthreader = this.sensor.DepthFrameSource.OpenReader();
        this.RawData = new ushort[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];
        this.SaveRawData = new ushort[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];
        this.ListCube = new List<GameObject>();

        this.cameraSpacePoints = new Windows.Kinect.CameraSpacePoint[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];

        if (!this.IsArchive)
        {
            this.depthreader.FrameArrived += depthreader_FrameArrived;
            this.sensor.Open();
        }
        if (this.IsArchive)
        {
            this.saveData = this.ReadDepth.GetComponent<ReadDepth>();
            
        }



       
    }

    void depthreader_FrameArrived(object sender, Windows.Kinect.DepthFrameArrivedEventArgs e)
    {
        using (var frame = e.FrameReference.AcquireFrame())
        {
            frame.CopyFrameDataToArray(this.RawData);
            this.SaveRawData = this.RawData;
            sensor.CoordinateMapper.MapDepthFrameToCameraSpace(this.RawData, this.cameraSpacePoints);
            
        }
        //if(ReadData != null)
        //{
        //    sensor.CoordinateMapper.MapDepthFrameToCameraSpace(this.ReadData, this.cameraSpacePoints2);
            
        //}
    }

    // Update is called once per frame
    void Update()
    {

        if (this.IsArchive)
        {
            this.sensor.CoordinateMapper.MapDepthFrameToCameraSpace(this.saveData.readData, this.cameraSpacePoints);
            //Debug.Log("pc");
        }

        int cubeCount = 0;
        int pointCount = 0;
        
        for (int i = 0; i < cameraSpacePoints.Length; i++)
        {
           
            //奥の壁排除
            if (this.cameraSpacePoints[i].Z > this.rangez.x && this.cameraSpacePoints[i].Z < this.rangez.y && this.ListCube.Count < this.maxCubeNum)
            {
               // Debug.Log("OK");
                //三次元位置に変更
                Vector3 point = new Vector3(this.cameraSpacePoints[i].X,this.cameraSpacePoints[i].Y,this.cameraSpacePoints[i].Z);
                
                //床排除と左右の壁排除                       
                if (point.y < this.roophight && point.y > -this.kinect.transform.position.y && point.x > this.rangex.x && point.x < this.rangex.y && pointCount % this.range == 0)
                {
                    
                    this.CubeControll(cubeCount, point);
                    //輪郭検索
                    //if (this.ChecK(i - 1) && this.ChecK(i + 1))
                    //{
                    //    輪郭の内側
                    //    if (pointCount % this.range == 0)
                    //    {
                    //        this.CubeControll(cubeCount, this.size, point);
                    //    }
                    //}
                    //else
                    //{
                    //    //輪郭
                    //    this.CubeControll(cubeCount, this.linesize, point);
                    //}
                                     
                    cubeCount++;
                }
                pointCount++;
            }

            

        }

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

    bool ChecK(int i)
    {
        if (RawData[i] > this.rangez.x && this.RawData[i] < this.rangez.y )
        {
            return true;
        }
        else { return false; }
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
    
    //パラメータ
    public void ChangeKinectPosition(Vector3 vec)
    {
        this.kinect.transform.position = vec;
    }

    


}
