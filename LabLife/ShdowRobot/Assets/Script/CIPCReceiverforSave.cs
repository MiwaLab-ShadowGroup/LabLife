using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;
using UnityEditor;

public class CIPCReceiverforSave : MonoBehaviour {

    public int serverPort;
    public string remoteIP;
    public int myPort;
    public string clientName;
    public int fps;

    CIPC_CS_Unity.CLIENT.CLIENT client;

    bool IsCIPC = false;

    byte[] data;

    bool RecState = false;

    string CIPCKey;

    public GameObject pointCloudShadow;
    PointCloud pointcloud;

    FPSAdjuster.FPSAdjuster FpsAd;

    Thread thread;
    Thread thread1;

    public bool CIPCSetup = false;

    BinaryWriter writer;
    string FolderPath;

    public bool OpenFileChoose = false;

    bool SaveStop;
    public string filename;


    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {
        this.pointcloud = pointCloudShadow.GetComponent<PointCloud>();


        this.FpsAd = new FPSAdjuster.FPSAdjuster();
        this.FpsAd.Fps = 30;
        this.FpsAd.Start();
    }

    // Update is called once per frame
    void Update()
    {

        if (CIPCSetup)
        {
            ConnectCIPC();
            this.CIPCSetup = false;

        }

        if (this.IsCIPC)
        {
            if (CIPCKey != null)
            {
                if (CIPCKey == "START")
                {
                    //Debug.Log("start");
                    this.RecState = true;
                    CIPCKey = null;
                }
                if (CIPCKey == "STOP")
                {
                    this.SaveStop = true;
                    //Debug.Log("stop");
                    CIPCKey = null;
                }
            }

        }

        if (OpenFileChoose)
        {
            FolderPath = EditorUtility.SaveFolderPanel("フォルダ選択", " ", " ");

            OpenFileChoose = false;

        }
        CIPCSave();

    }

    void GetData()
    {
        
        try
        {

            while (true)
            {
                if (IsCIPC && this.client.IsAvailable > 0)
                {
                    this.client.Update(ref this.data);
                    UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                    dec.Source = this.data;

                    //データ取得
                    CIPCKey = dec.get_string();
                }
            }

        }
        catch
        {

        }
       
    }

    void OnApplicationQuit()
    {
        if (this.client != null)
        {
            this.client.Close();
        }
        if (this.thread != null)
        {
            thread.Abort();

        }
        if(thread1 != null)
        {
            thread1.Abort();
        }
    }

    public void ConnectCIPC()
    {
        try
        {

            this.thread = new Thread(new ThreadStart(GetData));
            this.thread.Start();

            this.client = new CIPC_CS_Unity.CLIENT.CLIENT(this.myPort, this.remoteIP, this.serverPort, this.clientName, this.fps);
            this.client.Setup(CIPC_CS_Unity.CLIENT.MODE.Receiver);
            this.IsCIPC = true;

        }
        catch
        {
            Debug.Log("Erorr:CIPCforSave");
        }

    }

    void CIPCSave()
    {
        if (RecState)
        {
            if (FolderPath != null)
            {
                thread1 = new Thread(new ThreadStart(Save));
                thread1.Start();
                Debug.Log("start1");
                RecState = false;
            }
        }
        
    }

    void Save()
    {
        try
        {
            this.writer = new BinaryWriter(File.OpenWrite(FolderPath + @"\" + filename));


            while (true)
            {
                this.FpsAd.Adjust();
                //Debug.Log(framecount);
                writer.Write(pointcloud.SaveRawData.Length);

                for (int i = 0; i < pointcloud.SaveRawData.Length; i++)
                {

                    writer.Write(pointcloud.SaveRawData[i]);
                }

                //framecount++;
                if (this.SaveStop)
                {
                    SaveStop = false;
                    break;

                }
            }
            Debug.Log("stop1");

            this.writer.Close();
        }
        catch
        {

        }
    }

}
