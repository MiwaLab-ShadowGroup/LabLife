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
    [HideInInspector]
    public List<LRFdataSet> list_data;

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
        this.list_data = new List<LRFdataSet>();


        this.FpsAd = new FPSAdjuster.FPSAdjuster();
        this.FpsAd.Fps = 30;
        this.FpsAd.Start();

    }

    // Update is called once per frame
    void Update()
    {
       

    }


    void GetData()
    {
        while (true)
        {
            if (this.client.IsAvailable > 3)
            {
                this.FpsAd.Adjust();
                List<Vector3> list_position = new List<Vector3>();
                List<int> local_list_id = new List<int>();
                int id = -1;
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
                        int myID = dec.get_int();
                        
                        if (this.IsSC)
                        {
                            x /= 1000;
                            z /= 1000;                           
                        }

                        Vector3 vec = new Vector3(x, 0, z);
                        if (id == myID)
                        {
                            vec = (vec + list_humanpos[list_humanpos.Count - 1]) / 2;
                            list_humanpos[list_humanpos.Count - 1] = vec;
                        }
                        else
                        {
                            list_position.Add(vec);
                            local_list_id.Add(myID);
                        }
                        id = myID;
                    }

                    this.list_humanpos = list_position;
                   
                }
                catch
                {

                }
                
                
                
            }
        }
        

    }
    void GetDataSet()
    {
        while (true)
        {
            if (this.client.IsAvailable > 3)
            {
                this.FpsAd.Adjust();
                List<Vector3> locallist_position = new List<Vector3>();
                List<LRFdataSet> locallist_data = new List<LRFdataSet>();
                int id = -1;
                try
                {

                    this.client.Update(ref this.data);
                    UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                    dec.Source = this.data;

                    //データ格納
                    int humanNum = dec.get_int();
                    //Debug.Log(humanNum);
                    for (int i = 0; i < humanNum; i++)
                    {
                        float x = (float)dec.get_double();
                        float z = (float)dec.get_double();
                        int myID = dec.get_int();

                        if (this.IsSC)
                        {
                            x /= 1000;
                            z /= 1000;
                        }

                        Vector3 vec = new Vector3(x, 0, z);

                        LRFdataSet set = new LRFdataSet();
                        set.id = myID;
                        set.pos = vec;
                        locallist_data.Add(set);

                        if (id == myID)
                        {
                            vec = (vec + locallist_position[locallist_position.Count - 1]) / 2;
                            locallist_position[locallist_position.Count - 1] = vec;
                        }
                        else
                        {
                            locallist_position.Add(vec);
                        }
                        
                        id = myID;
                    }
                    this.list_data = locallist_data;
                    this.list_humanpos = locallist_position;
                    
                }
                catch
                {

                }


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


            this.thread = new Thread(new ThreadStart(this.GetDataSet));
            this.thread.Start();
        }
        catch
        {
            Debug.Log("Erorr:CIPC");
        }

    }
}
