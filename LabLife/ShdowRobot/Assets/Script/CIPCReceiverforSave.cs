using UnityEngine;
using System.Collections;
using System.Threading;

public class CIPCReceiverforSave : MonoBehaviour {

    public int serverPort;
    public string remoteIP;
    public int myPort;
    public string clientName;
    public int fps;

    CIPC_CS_Unity.CLIENT.CLIENT client;

    bool IsCIPC;


    byte[] data;

    bool RecState;

    string CIPCKey;

    SaveDepth savedepth;

    Thread thread;

    public bool CIPCSetup = false;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {
        this.IsCIPC = false;

        this.RecState = false;

        this.thread = new Thread(new ThreadStart(GetData));
        this.thread.Start();
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
            if(CIPCKey != null)
            {
                if (CIPCKey == "START")
                {
                    Debug.Log("start");
                    this.RecState = true;
                }
                if (CIPCKey == "STOP")
                {

                    Debug.Log("stop");
                    this.RecState = false;
                }
            }
            
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
        if (this.client != null){
            this.client.Close();
        }
        if(this.thread != null)
        {
            thread.Abort();

        }
    }

    public void ConnectCIPC()
    {
        try
        {
           
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

        }
        if (!RecState)
        {

        }

    }
}
