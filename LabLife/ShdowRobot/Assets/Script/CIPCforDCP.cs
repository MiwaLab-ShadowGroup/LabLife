using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CIPCforDCP : MonoBehaviour {

    public int serverPort;
    public string remoteIP;
    public int myPort;
    public string clinteName;
    public int fps;


    public int HumanNum;
    public GameObject PointCloudShadow;
    PointCloud pointCloud;
    CIPC_CS_Unity.CLIENT.CLIENT client;
    byte[] data;

    [HideInInspector]


    bool IsCIPC;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {
        this.IsCIPC = false;
        this.pointCloud = this.PointCloudShadow.GetComponent<PointCloud>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.IsCIPC)
        {
            this.SendData();

        }

    }

    void SendData()
    {
        try
        {

            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += this.HumanNum;
            for (int i = 0; i < this.HumanNum; i++)
            {
                enc += this.pointCloud.centerPos.x;
                enc += this.pointCloud.centerPos.y;
                enc += this.pointCloud.centerPos.z;

            }
            var bytes = enc.data;
            this.client.Update(ref bytes);
        }
        catch
        {
            Debug.Log("Error:SendDCPData");
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
            this.client.Setup(CIPC_CS_Unity.CLIENT.MODE.Sender);
            this.IsCIPC = true;
            Debug.Log("CIPCforDCP");
        }
        catch
        {
            Debug.Log("Erorr:CIPC");
        }

    }
}
