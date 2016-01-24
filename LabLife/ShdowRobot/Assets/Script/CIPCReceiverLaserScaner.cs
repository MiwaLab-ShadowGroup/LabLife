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
    public MonoBehaviour saverobot;

    struct _set
    {
        public int id;
        public Vector3 pos;
    }

    public GameObject cube;
    GameObject[] cubes;
    public GameObject text;
    GameObject[] texts;
    SaveRobotMT srMT;

    CIPC_CS_Unity.CLIENT.CLIENT client;
    byte[] data;

    [HideInInspector]
    public List<Vector3> list_humanpos;
    List<_set> list_data;

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
        this.list_data = new List<_set>();

        this.srMT = this.saverobot.GetComponent<SaveRobotMT>(); 

        this.FpsAd = new FPSAdjuster.FPSAdjuster();
        this.FpsAd.Fps = 30;
        this.FpsAd.Start();

        this.cubes = new GameObject[3];
        this.texts = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            this.cubes[i] = Instantiate(this.cube);
            this.texts[i] = Instantiate(this.text);
        }
        

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
        if (!this.srMT.IsStart)
        {
           // this.list_humanpos = new List<Vector3>();
        }

        if(this.list_data.Count > 0)
        {
            for (int i =0; i< this.list_data.Count; i++)
            {
                if (i < this.cubes.Length)
                {
                    this.cubes[i].transform.position = this.list_data[i].pos;
                    this.texts[i].transform.position = this.list_data[i].pos + new Vector3(0,0.5f,0);
                    this.texts[i].GetComponent<TextMesh>().text = this.list_data[i].id.ToString();
                }
                
            }
            if(this.list_data.Count < this.cubes.Length)
            {
                for (int i = this.list_data.Count; i < this.cubes.Length; i++)
                {
                    this.cubes[i].transform.position = new Vector3(0, -100, 0);
                    this.texts[i].transform.position = new Vector3(0, -100, 0);
                }
            }
        }

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
                            vec = ( vec +  list_humanpos[list_humanpos.Count - 1] ) / 2;
                            list_humanpos[list_humanpos.Count - 1] = vec;
                            
                        }
                        else
                        {
                            list_position.Add(vec);
                            local_list_id.Add(myID);
                        }


                        id = myID;
                    }
                    
                    this.srMT.save();
                }
                catch
                {

                }
                

                this.list_humanpos = list_position;
                
            }
        }
        

    }
    void GetData2()
    {
        while (true)
        {
            if (this.client.IsAvailable > 3)
            {
                this.FpsAd.Adjust();
                List<_set> list_position = new List<_set>();
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
                            vec = (vec + list_position[list_position.Count - 1].pos) / 2;
                            _set set = new _set();
                            set.id = myID;
                            set.pos = vec;
                            list_position[list_position.Count - 1] = set;
                        }
                        else
                        {
                            _set set = new _set();
                            set.id = myID;
                            set.pos = vec;
                            list_position.Add(set);
                            
                        }


                        id = myID;
                    }
                    this.list_data = list_position;
                    this.srMT.save();
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

            //Save 
            if(this.saverobot != null)
            {
                //this.srMT.IsStart = true;
            }

            this.thread = new Thread(new ThreadStart(this.GetData2));
            this.thread.Start();
        }
        catch
        {
            Debug.Log("Erorr:CIPC");
        }

    }
}
