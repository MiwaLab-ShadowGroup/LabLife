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

    public ParticleSystem particleSystem;


    // PARTICLE SYSTEM
    private ParticleSystem.Particle[] particles;
    List<ParticleSystem.Particle> list_particles;
    // DRAW CONTROL
    Color color;
    public int maxCubeNum = 5000;
    //パラメータ
    public Vector3 size { get; set; }
    public Vector3 linesize { get; set; }
    public Vector2 rangex;
    public Vector2 rangez;
    Vector3 kinectposition { get; set; }
    public int range;

    public int particulsnum;

    // Use this for initialization
    void Start()
    {
        this.sensor = Windows.Kinect.KinectSensor.GetDefault();
        this.depthreader = this.sensor.DepthFrameSource.OpenReader();
        this.depthreader.FrameArrived += depthreader_FrameArrived;
        this.RawData = new ushort[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];

        // particles to be drawn
        particles = new ParticleSystem.Particle[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];
        this.list_particles = new List<ParticleSystem.Particle>();
        this.ListCube = new List<GameObject>();

        this.cameraSpacePoints = new Windows.Kinect.CameraSpacePoint[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];
        this.sensor.Open();

        this.color = Color.red;
        this.size = new Vector3(0.03f, 0.03f, 0.03f);
        this.linesize = new Vector3(0.01f, 0.01f, 0.01f);

        //パラメータ初期化
        this.kinectposition = new Vector3(0f, 0.65f, 0f);
        //this.range = 10;
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
        int pointCount = 0;
        
        for (int i = 0; i < cameraSpacePoints.Length; i++)
        {
            //奥の壁排除
            if (this.cameraSpacePoints[i].Z > this.rangez.x && this.cameraSpacePoints[i].Z < this.rangez.y && this.ListCube.Count < this.maxCubeNum)
            {
                //三次元位置に変更
                Vector3 point = new Vector3(
                    this.cameraSpacePoints[i].X,
                    this.cameraSpacePoints[i].Y,
                    this.cameraSpacePoints[i].Z);
                //床排除と左右の壁排除                       
                if (point.y > -this.kinectposition.y && point.x > this.rangex.x && point.x < this.rangex.y && pointCount%this.range == 0)
                {
                    
                    this.CubeControll(cubeCount, this.size, point);
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
                    //    輪郭の外
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
    void CubeControll(int cubeCount, Vector3 size, Vector3 point)
    {
        if (cubeCount < this.ListCube.Count)
        {
            this.ListCube[cubeCount].transform.localPosition = point;
            //this.ListCube[cubeCount].transform.localScale = size;
        }
        else
        {
            this.ListCube.Add(Instantiate(this.cube));
            this.ListCube[this.ListCube.Count - 1].transform.parent = this.particleSystem.transform;
            this.ListCube[this.ListCube.Count - 1].transform.localPosition = point;
            //this.ListCube[cubeCount].transform.localScale = size;
        }
    }
    //パラメータ
    public void ChangeKinectPosition(Vector3 vec)
    {
        this.kinectposition = vec;
        this.particleSystem.transform.position = vec;
    }

}
