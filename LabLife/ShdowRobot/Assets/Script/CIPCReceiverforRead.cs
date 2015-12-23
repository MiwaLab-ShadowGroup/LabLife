using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEditor;

public class NewBehaviourScript : MonoBehaviour {


    BinaryReader reader;
    public ushort[] readData;
    int datalength;
    //public string ReadFileName;

    bool Isreader = true;
    Thread thread;
    Thread CIPCthread;


    FPSAdjuster.FPSAdjuster FpsAd;

    public bool OpenFileChoose = false;

    string FilePath;

    public bool ReadStart = false;

    public bool ReadStop = false;

    bool IsStart = false;

    CIPC_CS_Unity.CLIENT.CLIENT client;

    public bool CIPCSetup = false;

    string CIPCKey;
    bool IsCIPC = false;

    public int serverPort;
    public string remoteIP;
    public int myPort;
    public string clientName;
    public int fps;

    byte[] data;



    // Use this for initialization
    void Start () {


        readData = new ushort[512 * 424];

    }

    // Update is called once per frame
    void Update () {

        if (OpenFileChoose)
        {
            FilePath = EditorUtility.OpenFilePanel("ファイル選択", "　", "　");

            if (FilePath != null)
            {
                OpenFileChoose = false;
            }
        }

        if (IsStart)
        {
            this.FpsAd = new FPSAdjuster.FPSAdjuster();
            this.FpsAd.Fps = 30;
            this.FpsAd.Start();

            this.reader = new BinaryReader(File.OpenRead(FilePath));
            this.thread = new Thread(new ThreadStart(this.ReadData));
            this.thread.Start();
            IsStart = false;
            Isreader = true;
            //Debug.Log("isstart");
        }

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
                    this.IsStart = true;
                    CIPCKey = null;
                }
                if (CIPCKey == "STOP")
                {
                    this.ReadStop = true;
                    //Debug.Log("stop");
                    CIPCKey = null;
                }
            }

        }
    }


    void ReadData()
    {
        //Debug.Log("ok");
        while (true)
        {
            FpsAd.Adjust();
            if (ReadStart)
            {
                if (Isreader)
                {
                    //Debug.Log("ok2");
                    this.datalength = this.reader.ReadInt32();

                    for (int i = 0; i < datalength; i++)
                    {
                        this.readData[i] = this.reader.ReadUInt16();

                    }

                    if (reader.PeekChar() == -1)
                    {
                        //Debug.Log("ok3");
                        reader.Close();
                        Isreader = false;
                    }

                    if (ReadStop)
                    {
                        Isreader = false;
                        ReadStop = false;
                    }
                    //Debug.Log("OK");

                }
                else
                {
                    if (reader != null)
                    {
                        reader.Close();

                    }

                    break;
                }

            }

        }
        //Debug.Log("thread");

        if (reader != null)
        {
            reader.Close();
        }
        if (FilePath != null)
        {
            FilePath = null;
        }

        //readData = new ushort[512 * 424];

        ReadStart = false;
        if (thread != null)
        {
            thread.Abort();
        }

    }

    void OnDestroy()
    {
        if (thread != null)
        {
            thread.Abort();

        }
        if (FilePath != null)
        {
            FilePath = null;
        }
        if (reader != null)
        {
            reader.Close();
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
}
