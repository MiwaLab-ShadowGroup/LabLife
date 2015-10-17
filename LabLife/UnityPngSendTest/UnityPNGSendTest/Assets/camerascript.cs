using UnityEngine;
using System.Collections;

public class camerascript : MonoBehaviour {
    public RenderTexture renderTexture;
    private Texture2D sendtexture;
	// Use this for initialization
	void Start ()
    {
        if(this.renderTexture == null)
        {
            return;
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
        this.sendtexture.ReadPixels(new Rect( 0,0,this.renderTexture.width, this.renderTexture.height),0,0);
        this.sendtexture.Apply();

        Debug.Log( this.sendtexture.EncodeToJPG().Length);
    }
}
