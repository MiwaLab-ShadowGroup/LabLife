using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using FPSAdjuster;

public class SendImage : MonoBehaviour {


    [System.Serializable]
    public struct _destination
    {
        public string IPadress;
        public int Port;
    }
    [System.Serializable]
    public struct _changeColors
    {
        public Color srcColor;
        public Color dstColor;
    }

    //public RenderTexture renderTexture;
    public List<RenderTexture> List_RenderTexture;
    public _destination[] destinations;
    public bool IsChangeColor;
    public _changeColors[] List_Colors;
    public List<Color[]> List_PixelColors;
    public bool IsdifColor;
    public Color backColor;
    public bool IsInvert;

    private List<UdpClient> list_client;
    private Texture2D sendtexture;
    private List<Texture2D> List_SendTexture;
    private byte[] data;
    Thread thread;
    FPSAdjuster.FPSAdjuster fpsAdjuster;

    // Use this for initialization
    void Start ()
    {
        this.list_client = new List<UdpClient>();
        if (this.List_RenderTexture.Count == 0)
        {
            return;
        }

        for (int i = 0; i < this.destinations.Length; i++)
        {
            this.list_client.Add(new UdpClient());


            if (this.destinations[i].Port == 0)
            {
                this.destinations[i].Port = 15000 + i;
            }

            if (this.destinations[i].IPadress == "")
            {
                this.destinations[i].IPadress = "127.0.0.1";
            }
        }
        this.sendtexture = new Texture2D(this.List_RenderTexture[0].width, this.List_RenderTexture[0].height);
        this.List_SendTexture = new List<Texture2D>();
        this.List_PixelColors = new List<Color[]>();
        foreach (var Rtexture in this.List_RenderTexture)
        {
            this.List_SendTexture.Add(new Texture2D(Rtexture.width, Rtexture.height));
            this.List_PixelColors.Add(new Color[Rtexture.width * Rtexture.height]);
        }
        
        this.thread = new Thread(new ThreadStart(this.SendJPG));
        this.thread.Start(); 
        this.fpsAdjuster = new FPSAdjuster.FPSAdjuster();
        this.fpsAdjuster.Fps = 30;
        this.fpsAdjuster.Start();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //画像がなければ戻る
        if (this.List_RenderTexture.Count  < 0)
        {
            return;
        }

        for (int i = 0; i < this.List_RenderTexture.Count; i++)
        {
            RenderTexture.active = this.List_RenderTexture[i];
            this.List_SendTexture[i].ReadPixels(new Rect(0, 0, this.List_RenderTexture[i].width, this.List_RenderTexture[i].height), 0, 0);
            this.List_PixelColors[i] = this.List_SendTexture[i].GetPixels();
        }

        this.sendtexture = this.List_SendTexture[0];

        if (this.IsdifColor || IsInvert || IsChangeColor || this.List_RenderTexture.Count >1 )
        {

            //Debug.Log("OK");
            Color[] colors = this.sendtexture.GetPixels();

            for (int i = 0; i < colors.Length; i++)
            { 
                //合成
                foreach(var piccolors in this.List_PixelColors)
                {
                    //colors[i] -= this.difColor;
                    float r = Mathf.Abs(piccolors[i].r - this.backColor.r);
                    float g = Mathf.Abs(piccolors[i].g - this.backColor.g);
                    float b = Mathf.Abs(piccolors[i].b - this.backColor.b);
                    if (b > 0.1f || g > 0.1f || r > 0.1f)
                    {          
                        colors[i] = piccolors[i];

                        break;
                    }
                }

                //背景を引く
                if (this.IsdifColor)
                {
                    //colors[i] -= this.difColor;
                    float r = Mathf.Abs(colors[i].r - this.backColor.r);
                    float g = Mathf.Abs(colors[i].g - this.backColor.g);
                    float b = Mathf.Abs(colors[i].b - this.backColor.b);

                    //Color color = color = this.difColor - colors[i]; 

                    if (b < 0.1f && g < 0.1f && r < 0.1f)
                    {
                        colors[i] = Color.white;
                    }

                }
                //色の変更
                if (this.IsChangeColor)
                {
                    for (int j = 0; j < this.List_Colors.Length; j++)
                    {
                        float r = Mathf.Abs(colors[i].r - this.List_Colors[j].srcColor.r);
                        float g = Mathf.Abs(colors[i].g - this.List_Colors[j].srcColor.g);
                        float b = Mathf.Abs(colors[i].b - this.List_Colors[j].srcColor.b);

                        //Color color = color = this.difColor - colors[i]; 

                        if (b < 0.1f && g < 0.1f && r < 0.1f)
                        {
                            colors[i] = this.List_Colors[j].dstColor;
                        }
                    }
                }
                //反転処理
                if (this.IsInvert)
                {
                    colors[i] = Color.white - colors[i];

                }

            }
            this.sendtexture.SetPixels(colors);
           

        }


        this.sendtexture.Apply();
        this.data = this.sendtexture.EncodeToJPG();

    }

    void SendJPG()
    {
        while (true)
        {

            this.fpsAdjuster.Adjust();


            try
            {
                //Debug.Log("send");
                for (int i = 0; i < this.list_client.Count; i++)
                {
                    this.list_client[i].Send(this.data, this.data.Length, this.destinations[i].IPadress, this.destinations[i].Port);
                }
            }
            catch { }
        }
        

    }

    void OnDestroy()
    {
        if(this.thread != null)
        {
            this.thread.Abort();

        }
    }


}

