using UnityEngine;
using System.Net.Sockets;
using System.Collections;
using System.Net;
using System.Threading;
public class AddTextrure : MonoBehaviour {

    public RenderTexture renderTexture;
    public RenderTexture renderTexture1;
    public Color backColor;
    public bool IsInvert;
    //通信
    public string IPAdress;
    public int portNumber;
    //public int serverPort;
    private Texture2D sendtexture;
    private Texture2D sendtexture1;

    private UdpClient client;
    

    void Start()
    {
        if (this.renderTexture == null)
        {
            return;
        }
        this.client = new UdpClient();
        this.sendtexture = new Texture2D(this.renderTexture.width, this.renderTexture.height);
        this.sendtexture1 = new Texture2D(this.renderTexture1.width, this.renderTexture1.height);

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
        

        this.sendtexture.ReadPixels(new Rect(0, 0, this.renderTexture.width, this.renderTexture.height), 0, 0);
        Color[] colors =  this.sendtexture.GetPixels();

        RenderTexture.active = this.renderTexture1;
        this.sendtexture1.ReadPixels(new Rect(0, 0, this.renderTexture.width, this.renderTexture.height), 0, 0);
        Color[] colors1 = this.sendtexture1.GetPixels();
        
        for (int i = 0; i < colors.Length; i++)
        {
            if (this.IsInvert)
            {
                Color color = colors[i] - (Color.white - this.backColor);
                Color color1 = colors1[i] - (Color.white - this.backColor);
                if (color.b > 0.000001f && color.g > 0.000001f && color.r > 0.000001f)
                {
                    //colors[i] = Color.black;
                }
                else if (color1.b > 0.000001f && color1.g > 0.000001f && color1.r > 0.000001f)
                {
                    colors[i] = colors1[i];
                }
                else
                {
                    colors[i] = Color.white;
                }
            }
            else
            {
                Color color = colors[i] - colors1[i];
                if (colors[i] != this.backColor)
                {
                    //colors[i] = Color.black;
                }
                else if (colors1[i] != this.backColor)
                {
                    colors[i] = colors1[i];
                }
                else
                {
                    colors[i] = Color.white;
                }
            }
           
            
        }

        this.sendtexture.SetPixels(colors);

        this.sendtexture.Apply();

        var bytes = this.sendtexture.EncodeToJPG();

        this.client.Send(bytes, bytes.Length, this.IPAdress, this.portNumber);

        //Debug.Log("OK");
    }

}
