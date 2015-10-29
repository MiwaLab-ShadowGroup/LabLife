using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Net;

public class camerascript : MonoBehaviour {
    public RenderTexture renderTexture;
    public string IPAdress;
    public int portNumber;
    private Texture2D sendtexture;
    private UdpClient client;
    // Use this for initialization
    void Start ()
    {
        if(this.renderTexture == null)
        {
            return;
        }
        this.client = new UdpClient();
        this.sendtexture = new Texture2D(this.renderTexture.width, this.renderTexture.height);
        if(this.portNumber == 0)
        {
            this.portNumber = 15000;
        }
        if(this.IPAdress == "")
        {
            this.IPAdress = "127.0.0.1";
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (this.renderTexture == null)
        {
            return;
        }
        RenderTexture.active = this.renderTexture;
        this.sendtexture.ReadPixels(new Rect( 0,0,this.renderTexture.width, this.renderTexture.height),0,0);
        this.sendtexture.Apply();
        var bytes = this.sendtexture.EncodeToJPG();
        Debug.Log(this.client.Send(bytes,bytes.Length,this.IPAdress,this.portNumber));
        
    }
}
