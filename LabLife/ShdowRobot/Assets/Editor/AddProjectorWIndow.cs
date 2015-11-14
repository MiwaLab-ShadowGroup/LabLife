using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AddProjectorWIndow : EditorWindow
{

    private Camera projector = null;
    List<ProjectorWindow> list_pw = new List<ProjectorWindow> ();
    [MenuItem("Camera/AddProjectorWIndow")]
    static void Init()
    {
        EditorWindow editorWindow = GetWindow(typeof(AddProjectorWIndow));
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.Show();
    }
    
    void OnGUI()
    {
        EditorGUILayout.LabelField("プロジェクタ用別窓追加");
        // 置換オブジェクト
        Camera newProjector = EditorGUILayout.ObjectField(projector, typeof(Camera), true) as Camera;
        projector = newProjector;
        
        // 置換実行ボタン
        GameObject[] selection = Selection.gameObjects;
        if (GUILayout.Button("Add Projector Window"))
        {
            if (this.CheckProjector(projector))
            {
                ProjectorWindow pw = new ProjectorWindow();
                pw.projector = projector;
                pw.SetUp();
                pw.autoRepaintOnSceneChange = true;
                pw.Show();
                this.list_pw.Add(pw);
            }
            
        }
       

    }

    bool CheckProjector(Camera obj)
    {
        for (int i = 0; i < this.list_pw.Count; i++)
        {
            if (obj == this.list_pw[i].projector)
            {
                return false;
                //break;
            }

        }
        return true;
    }
    void OnDestroy()
    {
        for (int i = 0; i < this.list_pw.Count; i++)
        {
            this.list_pw[i].Close();
        }
    }
}