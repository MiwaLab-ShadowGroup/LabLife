using UnityEngine;
using System.Collections;
using System.Threading;

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

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {

        this.robotLight = this.robot.transform.FindChild("RobotLight").gameObject;
        this.IsCIPC = false;

        this.thread = new Thread(new ThreadStart(this.Data));
        thread.Start();
        
    }

    // Update is called once per frame
    void Data()
    {
        try
        {
            while (true)
            {
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
        catch { }
        

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
            this.robotPos = this.robot.transform.position;
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += this.robotPos.x;
            enc += this.robotPos.y;
            enc += this.robotPos.z;
            enc += this.robotLightPos.x;
            enc += this.robotLightPos.y;
            enc += this.robotLightPos.z;
            this.data = enc.data;
            this.client.Update(ref this.data);
            Debug.Log("Send");

        }
        catch
        {
            Debug.Log("Error:ReceiveData");

        }
    }
    void OnAppLicatinQuit()
    {
        this.client.Close();
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
            Debug.Log("CIPCforRobotSync");
        }
        catch
        {
            Debug.Log("Erorr:CIPC");
        }

    }
}
