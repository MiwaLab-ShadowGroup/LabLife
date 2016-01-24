using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;

public class TimeDelaytest : MonoBehaviour {

    public RenderTexture renderTexture;
    public string IPAdress;
    public int portNumber;
    private Texture2D readtexture;
    private Texture2D sendtexture;
    List<Color[]> list_color;
    private UdpClient client;

    // Use this for initialization
    void Start()
    {
        if (this.renderTexture == null)
        {
            return;
        }
        this.client = new UdpClient();
        this.list_color = new List<Color[]>();
        this.sendtexture = new Texture2D(this.renderTexture.width, this.renderTexture.height);
        this.readtexture = new Texture2D(this.renderTexture.width, this.renderTexture.height);
        if (this.portNumber == 0)
        {
            this.portNumber = 15000;
        }
        if (this.IPAdress == "")
        {
            this.IPAdress = "127.0.0.1";
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.renderTexture == null)
        {
            return;
        }
        
        RenderTexture.active = this.renderTexture;
        this.readtexture.ReadPixels(new Rect(0, 0, this.renderTexture.width, this.renderTexture.height), 0, 0);

        Color[] colors = this.readtexture.GetPixels();
        this.list_color.Add(colors);

        if(this.list_color.Count > 100)
        {
            this.sendtexture.SetPixels(this.list_color[0]);
            this.sendtexture.Apply();
            var bytes = this.sendtexture.EncodeToJPG();
            Debug.Log(this.client.Send(bytes, bytes.Length, this.IPAdress, this.portNumber));
            this.list_color.RemoveAt(0);
        }
        

    }
}
