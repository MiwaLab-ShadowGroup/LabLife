using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;

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


    public RenderTexture renderTexture;
    public _destination[] destinations;
    public bool IsChangeColor;
    public _changeColors[] List_Colors;
    public bool IsdifColor;
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

        if (this.IsdifColor || IsInvert || IsChangeColor)
        {

            //バック処理
            Color[] colors = this.sendtexture.GetPixels();
            for (int i = 0; i < colors.Length; i++)
            {

                if (this.IsdifColor)
                {
                    //colors[i] -= this.difColor;
                    float r = Mathf.Abs(colors[i].r - this.difColor.r);
                    float g = Mathf.Abs(colors[i].g - this.difColor.g);
                    float b = Mathf.Abs(colors[i].b - this.difColor.b);

                    //Color color = color = this.difColor - colors[i]; 

                    if (b < 0.1f && g < 0.1f && r < 0.1f)
                    {
                        colors[i] = Color.white;
                    }

                }
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


                if (this.IsInvert)
                {
                    colors[i] = Color.white - colors[i];

                }

            }
            this.sendtexture.SetPixels(colors);

        }



        this.sendtexture.Apply();



        var bytes = this.sendtexture.EncodeToJPG();

        try
        {
            for (int i = 0; i < this.list_client.Count; i++)
            {
                this.list_client[i].Send(bytes, bytes.Length, this.destinations[i].IPadress, this.destinations[i].Port);
            }
        }
        catch { }

    }

}

