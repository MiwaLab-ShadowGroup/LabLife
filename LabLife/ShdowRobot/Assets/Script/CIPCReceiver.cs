using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CIPCReceiver : MonoBehaviour 
{

    public int serverPort;
    public string remoteIP;
    public int myPort;
    public string clinteName;
    public int fps;
    CIPC_CS_Unity.CLIENT.CLIENT client;
    byte[] data;

    [HideInInspector]
    public List<Human> List_Humans;

    bool IsCIPC;

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start () 
    {
        this.IsCIPC = false;
        this.List_Humans = new List<Human>();     
	}
	
	// Update is called once per frame
	void Update () {
        if (this.IsCIPC)
        {
            if (this.client.IsAvailable > 3) this.GetData();
      
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

                }

                this.List_Humans.Add(human);
            }
        }
        catch
        {
            Debug.Log("Error:ReceiveData");
        }

    
    }

    void OnDestroy()
    {
        if (this.client != null)
        {
            this.client.Close();
        }

        
    }

    public void ConnectCIPC()
    {
        try
        {
            this.client = new CIPC_CS_Unity.CLIENT.CLIENT(this.myPort, this.remoteIP, this.serverPort, this.clinteName, this.fps);
            this.client.Setup(CIPC_CS_Unity.CLIENT.MODE.Receiver);
            this.IsCIPC = true;
            Debug.Log("CIPCforKinect");
        }
        catch
        {
            Debug.Log("Erorr:CIPC");
        }
        
    }

  
}
