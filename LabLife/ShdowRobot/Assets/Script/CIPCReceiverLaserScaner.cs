using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class CIPCReceiverLaserScaner : MonoBehaviour 
{

    public int serverPort;
    public string remoteIP;
    public int myPort;
    public string clinteName;
    public int fps;
    public bool IsSC = false;

    CIPC_CS_Unity.CLIENT.CLIENT client;
    byte[] data;

    [HideInInspector]
    public List<Vector3> list_humanpos;

    Thread thread;
    FPSAdjuster.FPSAdjuster FpsAd;




    bool IsCIPC;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Use this for initialization
    void Start()
    {
        this.IsCIPC = false;
        this.list_humanpos = new List<Vector3>();


        this.FpsAd = new FPSAdjuster.FPSAdjuster();
        this.FpsAd.Fps = 30;
        this.FpsAd.Start();
    }

    // Update is called once per frame
    void Update()
    {
        //if (this.IsCIPC)
        //{
        //    if (this.client.IsAvailable > 3) 
        //        this.list_humanpos = this.GetData();
        //    //Debug.Log("CIPC");       
        //}

    }


    void GetData()
    {
        while (true)
        {
            if (this.client.IsAvailable > 3)
            {
                this.FpsAd.Adjust();
                List<Vector3> list_position = new List<Vector3>();

                try
                {

                    this.client.Update(ref this.data);
                    UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                    dec.Source = this.data;

                    //データ格納
                    int humanNum = dec.get_int();
                    for (int i = 0; i < humanNum; i++)
                    {
                        float x = (float)dec.get_double();
                        float z = (float)dec.get_double();
                        
                        if (this.IsSC)
                        {
                            int id = dec.get_int();
                            list_position.Add(new Vector3(x / 1000, 0, z / 1000));
                        }
                        else { list_position.Add(new Vector3(x, 0, z)); }


                    }
                    //if (list_position.Count > 0) Debug.Log(list_position[0]); 
                }
                catch
                {

                }
                this.list_humanpos = list_position;
            }
        }
        

    }

    void OnDestroy()
    {
        if (this.client != null)
        {
            this.client.Close();

        }
        if(this.thread != null)
        {
            this.thread.Abort();
        }

    }

    public void ConnectCIPC()
    {
        try
        {
            //this.client = new CIPC_CS_Unity.CLIENT.CLIENT(myport, ip, serverport, "ShadowLS", 30);  
            this.client = new CIPC_CS_Unity.CLIENT.CLIENT(this.myPort, this.remoteIP, this.serverPort, this.clinteName, this.fps); 
            this.client.Setup(CIPC_CS_Unity.CLIENT.MODE.Receiver);
            this.IsCIPC = true;
            Debug.Log("CIPCforLaserScaner");

            this.thread = new Thread(new ThreadStart(this.GetData));
            this.thread.Start();
        }
        catch
        {
            Debug.Log("Erorr:CIPC");
        }

    }
}
