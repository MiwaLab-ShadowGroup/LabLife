using UnityEngine;
using System.Collections;

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


    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {
        this.IsCIPC = false;

        this.RecState = false;
    }

    // Update is called once per frame
    void Update()
    {

        if (this.IsCIPC)
        {
            GetData();      

            if(CIPCKey == "START")
            {
                this.RecState = true;
            }
            if(CIPCKey == "STOP")
            {
                this.RecState = false;
            }
        }
        CIPCSave();

    }

    string GetData()
    {
        
        try
        {

            this.client.Update(ref this.data);
            UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            dec.Source = this.data;

            //データ取得
            CIPCKey = dec.get_string();
            
        }
        catch
        {

        }
        return CIPCKey;

    }



    void OnAppLicatinQuit()
    {
        this.client.Close();
    }

    public void ConnectCIPC()
    {
        try
        {
           
            this.client = new CIPC_CS_Unity.CLIENT.CLIENT(this.myPort, this.remoteIP, this.serverPort, this.clientName, this.fps);
            this.client.Setup(CIPC_CS_Unity.CLIENT.MODE.Receiver);
            this.IsCIPC = true;
           // Debug.Log("CIPCforSave");
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
