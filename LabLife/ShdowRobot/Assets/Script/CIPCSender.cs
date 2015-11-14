using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CIPCSender : MonoBehaviour {
    public int serverPort;
    public string remoteIP;
    public int myPort;

    CIPC_CS_Unity.CLIENT.CLIENT client;
    byte[] data;
    AizawaModel generator;
    List<Human> List_SendHuman;

    // Use this for initialization
    void Start()
    {
        this.client = new CIPC_CS_Unity.CLIENT.CLIENT(this.myPort, this.remoteIP, this.serverPort);
        this.client.Setup(CIPC_CS_Unity.CLIENT.MODE.Sender);
        this.generator = GameObject.FindGameObjectWithTag("Generator").GetComponent<AizawaModel>();
        this.List_SendHuman = new List<Human>();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.generator.List_Humans != null)
            this.SendData();
    }

    void SendData()
    {
        try
        {
            this.List_SendHuman.Clear();
            this.List_SendHuman = this.generator.List_Humans;
            //this.List_SendHuman.Add(this.generator.robotBone);

            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();

            enc += (byte)6;
            enc += (byte)this.generator.List_Humans.Count;

            for (int i = 0; i < this.generator.List_Humans.Count; i++)
            {
                enc += (byte)this.List_SendHuman[i].numofBone;

                for (int j = 0; j < this.List_SendHuman[i].numofBone; j++)
                {

                    enc += this.List_SendHuman[i].bones[j].dimensiton;
                    enc += this.List_SendHuman[i].bones[j].position.x;
                    enc += this.List_SendHuman[i].bones[j].position.y;
                    enc += this.List_SendHuman[i].bones[j].position.z;

                    enc += this.List_SendHuman[i].bones[j].quaternion.x;
                    enc += this.List_SendHuman[i].bones[j].quaternion.y;
                    enc += this.List_SendHuman[i].bones[j].quaternion.z;
                    enc += this.List_SendHuman[i].bones[j].quaternion.w;

                    enc += this.List_SendHuman[i].bones[j].IsTracking;
                }
            }
        }
        catch
        {
            Debug.Log("Error:SendData");
        }
        

        
    }

    void OnAppLicatinQuit()
    {
        this.client.Close();
    }
}
