using UnityEngine;
using System.Collections;
using System.Threading;
using FPSAdjuster;

public class CIPCRobotSync : MonoBehaviour {

    public enum _Mode { Reciever, Sender,}
    public _Mode mode;
    public int serverPort;
    public string remoteIP;
    public int myPort;
    public string clinteName;
    public int fps;
    CIPC_CS_Unity.CLIENT.CLIENT client;
    byte[] data;
    public GameObject robot;

    GameObject robotLight;
    [HideInInspector]
    public Vector3 robotPos;
    [HideInInspector]
    public Vector3 robotLightPos;
    bool IsCIPC;
    public bool IsStop;

    Thread thread;
    FPSAdjuster.FPSAdjuster fpsAdjuster;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {
        this.robotLight = this.robot.transform.FindChild("RobotLight").gameObject;
        this.IsCIPC = false;
        //Debug.Log("OK");

        this.thread = new Thread(new ThreadStart(this.Data));
        this.fpsAdjuster = new FPSAdjuster.FPSAdjuster();
        this.fpsAdjuster.Fps = this.fps;
        this.fpsAdjuster.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.mode == _Mode.Sender)
        {
            this.robotPos = this.robot.transform.position;
        }

    }

    void Data()
    {
        while (true)
        {
            this.fpsAdjuster.Adjust();
            if (this.IsCIPC)
            {
                switch (this.mode)
                {
                    case _Mode.Reciever: this.GetData(); break;
                    case _Mode.Sender: this.SendData(); break;
                }

            }
            if (this.IsStop) break;
        }
        
    }

    void GetData()
    {
        try
        {
            if (this.client.IsAvailable > 3)
            {
                this.client.Update(ref this.data);
                UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                dec.Source = this.data;
                this.robotPos.x = dec.get_float();
                this.robotPos.y = dec.get_float();
                this.robotPos.z = dec.get_float();
                this.robotLightPos.x = dec.get_float();
                this.robotLightPos.y = dec.get_float();
                this.robotLightPos.z = dec.get_float();
            }
        }
        catch
        {
            Debug.Log("Error:ReceiveData");
        }


    }
    void SendData()
    {
        try
        {
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += this.robotPos.x;
            enc += this.robotPos.y;
            enc += this.robotPos.z;
            enc += this.robotLightPos.x;
            enc += this.robotLightPos.y;
            enc += this.robotLightPos.z;
            this.data = enc.data;
            this.client.Update(ref this.data);

        }
        catch
        {
            Debug.Log("Error:SendData");

        }
    }

    void OnDestroy()
    {
        if (this.client != null)
        {
            this.client.Close();
            this.thread.Abort();
        }
        

    }

    public void ConnectCIPC()
    {
        

        try
        {
            this.client = new CIPC_CS_Unity.CLIENT.CLIENT(this.myPort, this.remoteIP, this.serverPort, this.clinteName, this.fps);

            switch (this.mode)
            {
                case _Mode.Reciever: this.client.Setup(CIPC_CS_Unity.CLIENT.MODE.Receiver); break;
                case _Mode.Sender: this.client.Setup(CIPC_CS_Unity.CLIENT.MODE.Sender); break;
            }

            this.IsCIPC = true;
            this.thread.Start();

            Debug.Log("CIPCforRobotSync");
        }
        catch
        {
            Debug.Log("Erorr:CIPC");
        }

    }
}
