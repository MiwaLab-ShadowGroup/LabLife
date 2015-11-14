using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CIPCReceiver : MonoBehaviour {

    public int serverPort;
    public string remoteIP;
    public int myPort;
    public string name;

    CIPC_CS_Unity.CLIENT.CLIENT client;
    byte[] data;
    public List<Human> List_Humans;

    //Test用
    Vector3 HumanPosition;
    Quaternion Quat;

    bool IsCIPC;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start () {
        this.IsCIPC = false;
        this.List_Humans = new List<Human>();

        /*
        try
        {
            this.client = new CIPC_CS_Unity.CLIENT.CLIENT(this.myPort, this.remoteIP, this.serverPort, this.name, 30);
            this.client.Setup(CIPC_CS_Unity.CLIENT.MODE.Receiver);
            this.List_Humans = new List<Human>();
            this.IsCIPC = true;
            Debug.Log("CIPC");
        }
        catch { }
        */
        //Test用
        //this.HumanPosition = new Vector3(0, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {
        if (this.IsCIPC)
        {
            if (this.client.IsAvailable > 3) this.GetData();
            //Debug.Log("CIPC");

            if (this.List_Humans.Count > 0)
            {
                this.MovePoint();
                //Debug.Log(this.HumanPosition.ToString());
                //this.List_Humans[0].bones[0].position = this.HumanPosition;
            }
        }
        
	}

    void GetData()
    {
        try
        {
            this.client.Update(ref this.data);
            UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            dec.Source = this.data;

            //データ格納
            this.List_Humans.Clear();

            int MaxofHuman = (int)dec.get_byte();
            int NumofHuman = (int)dec.get_byte();

            this.List_Humans = new List<Human>();

            for (int i = 0; i < NumofHuman; i++)
            {
                Human human = new Human();
                human.id = i;
                human.numofBone = (int)dec.get_byte();
                human.bones = new Bone[human.numofBone];

                for (int j = 0; j < human.numofBone; j++)
                {
                    Bone bone = new Bone();

                    bone.dimensiton = (int)dec.get_byte();
                    bone.position.x = dec.get_float();
                    bone.position.y = dec.get_float();
                    bone.position.z = dec.get_float();
                    bone.quaternion.x = dec.get_float();
                    bone.quaternion.y = dec.get_float();
                    bone.quaternion.z = dec.get_float();
                    bone.quaternion.w = dec.get_float();
                    bone.IsTracking = dec.get_byte();

                    human.bones[j] = bone;

                    //if(j == 0) Debug.Log(bone.position.ToString()) ;
                }

                this.List_Humans.Add(human);
            }
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

    void MovePoint()
    {
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.HumanPosition += Vector3.forward / 10 ;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            this.HumanPosition += Vector3.back / 10;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            this.HumanPosition += Vector3.right / 10;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            this.HumanPosition += Vector3.left / 10;
        }

        if (Input.GetKey(KeyCode.W))
        {
            this.Quat *= Quaternion.AngleAxis(1, Vector3.up);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.Quat *= Quaternion.AngleAxis(-1, Vector3.up);
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.Quat *= Quaternion.AngleAxis(1, Vector3.up);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.Quat *= Quaternion.AngleAxis(-1, Vector3.up);
        } 
    }

    public void ConnectCIPC(int myport, string ip, int serverport)
    {
        try
        {
            this.client = new CIPC_CS_Unity.CLIENT.CLIENT(myport, ip, serverport, "Shadow", 30);
            this.client.Setup(CIPC_CS_Unity.CLIENT.MODE.Receiver);
            this.IsCIPC = true;
            Debug.Log("CIPC");
        }
        catch
        {
            Debug.Log("Erorr:CIPC");
        }
        
    }

    /*
    void OnGUI()
    {
        int myPort;
        int serverPort;
        string IPAdress;
        
        myPort = int.Parse(GUI.TextField(new Rect(0,0,100,10), "51000")) ;
        IPAdress =  GUI.TextField(new Rect(0, 0, 10, 100), "127.0.0.1");
        serverPort = int.Parse(GUI.TextField(new Rect(0,0,10,100),"50000"));
    }
     * */
}
