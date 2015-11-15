using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Test : MonoBehaviour {
    //public GameObject cube;
    private List<GameObject> ListCube = new List<GameObject>();
    public Windows.Kinect.KinectSensor sensor;
    private ushort[] RawData;
    private Windows.Kinect.CameraSpacePoint[] cameraSpacePoints;
    public Windows.Kinect.DepthFrameReader depthreader { get; set; }

    public ParticleSystem particleSystem;


    // PARTICLE SYSTEM
    private ParticleSystem.Particle[] particles;
    // DRAW CONTROL
    public Color color = Color.white;
    public float size = 0.2f;
    public float scale = 10f;


    // Use this for initialization
	void Start () {

        

        this.sensor = Windows.Kinect.KinectSensor.GetDefault();
        this.depthreader = this.sensor.DepthFrameSource.OpenReader();
        this.depthreader.FrameArrived += depthreader_FrameArrived;
        this.RawData = new ushort[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];

        // particles to be drawn
        particles = new ParticleSystem.Particle[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];
        
        this.cameraSpacePoints = new Windows.Kinect.CameraSpacePoint[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];
        this.sensor.Open();


    }

    void depthreader_FrameArrived(object sender, Windows.Kinect.DepthFrameArrivedEventArgs e)
    {
        using (var frame = e.FrameReference.AcquireFrame())
        {            
            frame.CopyFrameDataToArray(this.RawData);
            sensor.CoordinateMapper.MapDepthFrameToCameraSpace(this.RawData, this.cameraSpacePoints);
            Debug.Log(cameraSpacePoints.Length + " " + particles.Length);
            for (int i = 0; i < cameraSpacePoints.Length; i++)
            {

                particles[i].position = new Vector3(cameraSpacePoints[i].X * scale, cameraSpacePoints[i].Y * scale, cameraSpacePoints[i].Z * scale);
                particles[i].color = color;
                particles[i].size = size;
                if (RawData[i] == 0) particles[i].size = 0;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        

        // update particle system

        particleSystem.SetParticles(this.particles, this.particles.Length);
	}

}
