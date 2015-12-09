using UnityEngine;
using System.Collections;




namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [AddComponentMenu("Image Effects/Color Adjustments/Sepia Tone")]
    public class Invert: ImageEffectBase
    {
        // Called by camera to apply image effect
        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination, material);
        }
    }
}

//public class TestShader : MonoBehaviour {

//    public Shader shader;
//    public Material material;

//    void Start()
//    {
//        this.material = new Material(shader);
//    }

//    void OnRenderImage(RenderTexture source, RenderTexture destination)
//    {
//        Graphics.Blit(source, destination, material);

//    }
//}
