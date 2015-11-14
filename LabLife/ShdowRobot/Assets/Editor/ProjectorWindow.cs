using UnityEngine;
using UnityEditor;

public class ProjectorWindow : EditorWindow
{
    public Camera projector;
    RenderTexture renderTexture;
    Rect preRect;
    [MenuItem("Camera/ProjectorWindow")]
    public void SetUp()
    {
        this.renderTexture = new RenderTexture(320, 240, 255);
        this.preRect = this.projector.rect;
        this.projector.rect = new Rect(0, 0, 1, 1);
        this.projector.targetTexture = this.renderTexture;
    }

    void OnGUI()
    {
        RenderTexture texture = this.projector.targetTexture;
        if (this.renderTexture != null)
        {

            GUI.DrawTexture(new Rect(0.0f, 0.0f, position.width, position.height), texture);

        }
        else Debug.Log("noimage");
    }

    void OnDestroy()
    {
        this.projector.rect = this.preRect;
        this.projector.targetTexture = null;
    }
}
