using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

public class SendImage : MonoBehaviour {


    public RenderTexture renderTexture;
    public string[] list_IP;
    public int[] list_PortNum;

    public bool IsdiColor;
    public Color difColor;
    public bool IsInvert;
    private List<UdpClient> list_client;
    private Texture2D sendtexture;

    // Use this for initialization
    void Start ()
    {
        this.list_client = new List<UdpClient>();
        if (this.renderTexture == null)
        {
            return;
        }

        for (int i = 0; i< this.list_IP.Length; i++)
        {
            this.list_client.Add(new UdpClient());


            if (this.list_PortNum[i] == 0)
            {
                this.list_PortNum[i] = 15000 + i;
            }
           
            if (this.list_IP[i] == "")
            {
                this.list_IP[i] = "127.0.0.1";
            }
        }
        this.sendtexture = new Texture2D(this.renderTexture.width, this.renderTexture.height);
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        if (this.renderTexture == null)
        {
            return;

        }

        RenderTexture.active = this.renderTexture;
        this.sendtexture.ReadPixels(new Rect(0, 0, this.renderTexture.width, this.renderTexture.height), 0, 0);

        if (this.IsdiColor)
        {
            //バック処理

            Color[] colors = this.sendtexture.GetPixels();
            for (int i = 0; i < colors.Length; i++)
            {
                //colors[i] -= this.difColor;
                Color color;
                if (this.IsInvert)
                {
                    color = colors[i] - (Color.white - this.difColor);
                }
                else { color = this.difColor - colors[i]; }

                if (color.b < 0.000001f && color.g < 0.000001f && color.r < 0.000001f)
                {
                    colors[i] = Color.white;
                }
            }
        }



        this.sendtexture.Apply();



        var bytes = this.sendtexture.EncodeToJPG();
        //this.CIPCclient.Update(ref bytes);

        try
        {
            for (int i = 0; i < this.list_client.Count; i++)
            {
                this.list_client[i].Send(bytes, bytes.Length, this.list_IP[i], this.list_PortNum[i]);
            }
        }
        catch { }

        //Debug.Log(this.client.Send(bytes, bytes.Length, this.IPAdress, this.portNumber));
    }

}

