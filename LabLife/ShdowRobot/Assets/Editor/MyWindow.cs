// Simple script that lets you render the main camera in an editor Window.
	
using UnityEngine;
using UnityEditor;
	
public class CameraViewer : EditorWindow {

    public Camera projector;
	RenderTexture renderTexture;

	[MenuItem("Camera/Camera viewer")]
	static void Init() {
		EditorWindow editorWindow = GetWindow(typeof(CameraViewer));
		editorWindow.autoRepaintOnSceneChange = true;
		editorWindow.Show();
	}
	public void Awake () {
		renderTexture = new RenderTexture((int)position.width, 
					(int)position.height, 
					(int)RenderTextureFormat.ARGB32 );
	}
	public void Update() {
        if (projector != null)
        {
            projector.targetTexture = renderTexture;
            projector.Render();
            projector.targetTexture = null;	
		}
		if(renderTexture.width != position.width || 
			renderTexture.height != position.height)
			renderTexture = new RenderTexture((int)position.width, 
							(int)position.height, 
							(int)RenderTextureFormat.ARGB32 );
	}
	void OnGUI() {
		GUI.DrawTexture( new Rect( 0.0f, 0.0f, position.width, position.height), renderTexture );	
	}
}