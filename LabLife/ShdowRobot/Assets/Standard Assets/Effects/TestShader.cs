using UnityEngine;
using System.Collections;

public class TestShader : MonoBehaviour {

    public Shader shader;

    Material material;

    void Start()
    {
        this.material = new Material(shader);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
            Graphics.Blit(source, destination, material);
        
    }
}
