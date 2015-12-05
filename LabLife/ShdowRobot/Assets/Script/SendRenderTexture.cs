using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Net;
using System.Threading;

public class SendRenderTexture : MonoBehaviour {

    public RenderTexture renderTexture;
    public Color difColor;
    public bool IsInvert;
    //通信
    public string IPAdress;
    public int portNumber;

    private Texture2D sendtexture;

    private UdpClient client;


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

        if (this.IPAdress == "")
        {
            this.IPAdress = "127.0.0.1";
        }

        
    }

    private void sendimage()
    {
      

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

        //バック処理

        Color[] colors = this.sendtexture.GetPixels();
        for (int i = 0; i < colors.Length; i++)
        {
            //colors[i] -= this.difColor;
            Color color;
            if (this.IsInvert)
            {
                color =  colors[i] - (Color.white - this.difColor );
            }
            else { color = this.difColor - colors[i]; }

            if (color.b < 0.000001f && color.g < 0.000001f && color.r < 0.000001f)
            {
                colors[i] = Color.white;
            }
        }



        this.sendtexture.SetPixels(colors);
        this.sendtexture.Apply();


        var bytes = this.sendtexture.EncodeToJPG();
        //this.CIPCclient.Update(ref bytes);

        this.client.Send(bytes, bytes.Length, this.IPAdress, this.portNumber);
        //Debug.Log(this.client.Send(bytes, bytes.Length, this.IPAdress, this.portNumber));
    }

}
