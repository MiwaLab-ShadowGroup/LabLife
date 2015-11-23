using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Net;
using System.Threading;


public class Lablife : MonoBehaviour
{
    public RenderTexture renderTexture;
    //通信
    public string IPAdress;
    public int portNumber;
    //public int serverPort;
    private Texture2D sendtexture;
    private UdpClient client;
    //private CIPC_CS_Unity.CLIENT.CLIENT CIPCclient;
    //private Thread thred;
    //private FPSAdjuster.FPSAdjuster fps;
    // Use this for initialization
    

    void Start()
    {
        if (this.renderTexture == null)
        {
            return;
        }
        this.client = new UdpClient();
        this.sendtexture = new Texture2D(this.renderTexture.width, this.renderTexture.height);
        if (this.portNumber == 0)
        {
            this.portNumber = 15000;
        }
        //if (this.serverPort == 0)
        //{
        //    this.serverPort = 50000;
        //}
        if (this.IPAdress == "")
        {
            this.IPAdress = "127.0.0.1";
        }
        
        
        //this.CIPCclient = new CIPC_CS_Unity.CLIENT.CLIENT(this.portNumber, this.IPAdress, this.serverPort);
        //this.CIPCclient.Setup(CIPC_CS_Unity.CLIENT.MODE.Sender);

        //this.fps = new FPSAdjuster.FPSAdjuster();
        //this.fps.Fps = 30;
        //this.fps.Start();
        //this.thred = new Thread (new ThreadStart(this.sendimage));
        //this.thred.Start();
    }

    private void sendimage()
    {
        //while (true)
        //{

        //    this.fps.Adjust();
        //}
        
    }

    // Update is called once per frame
    void Update()
    {

        if (this.renderTexture == null)
        {
            return;
        }
        
        RenderTexture.active = this.renderTexture;
        this.sendtexture.ReadPixels(new Rect(0, 0, this.renderTexture.width, this.renderTexture.height), 0, 0);
        this.sendtexture.Apply();
        var bytes = this.sendtexture.EncodeToJPG();
        //this.CIPCclient.Update(ref bytes);
        
        this.client.Send(bytes, bytes.Length, this.IPAdress, this.portNumber);
        //Debug.Log(this.client.Send(bytes, bytes.Length, this.IPAdress, this.portNumber));
    }

    
}
