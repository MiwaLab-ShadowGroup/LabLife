using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

public class CIPCReceiverforSave : MonoBehaviour {

    public int serverPort;
    public string remoteIP;
    public int myPort;
    public string clientName;
    public int fps;

    ISave[] List_Sctipt;

    CIPC_CS_Unity.CLIENT.CLIENT client;
    bool IsCIPC = false;
    byte[] data;
    bool RecState = false;
    string CIPCKey;
    public GameObject pointCloudShadow;
    PointCloud pointcloud;
    FPSAdjuster.FPSAdjuster FpsAd;

    Thread CIPCthread;
    Thread Savethread;

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

        this.Set_ListScript();
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
                    this.Set_StartSave(true);
                    CIPCKey = null;
                }
                if (CIPCKey == "STOP")
                {
                    this.SaveStop = true;
                    this.Set_StartSave(false);
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

    void OnDestroy()
    {
        if (this.client != null)
        {
            this.client.Close();
        }
        if (this.CIPCthread != null)
        {
            CIPCthread.Abort();

        }
        if(Savethread != null)
        {
            Savethread.Abort();
        }

        if(writer != null)
        {
            writer.Close();

        }
    }

    public void ConnectCIPC()
    {
        try
        {

            this.CIPCthread = new Thread(new ThreadStart(GetData));
            this.CIPCthread.Start();

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
                Savethread = new Thread(new ThreadStart(Save));
                Savethread.Start();
                Debug.Log("start");
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
            Debug.Log("stop");

            this.writer.Close();
        }
        catch
        {

        }
    }

    void Set_ListScript()
    {
        Component[] components = this.GetComponents(typeof(ISave));
        this.List_Sctipt = new ISave[components.Length];
        for(int i = 0; i < this.List_Sctipt.Length; i++)
        {
            this.List_Sctipt[i] = components[i] as ISave;
        }
        
        
    }
    void Set_StartSave(bool isStart)
    {
        foreach(var script in this.List_Sctipt)
        {
            script.SetIsStart = isStart;
        }
    }
}
