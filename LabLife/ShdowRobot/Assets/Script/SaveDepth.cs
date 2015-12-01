using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEditor;

public class SaveDepth : MonoBehaviour {

    public GameObject pointCloudShadow;
    public bool IsSaveStop = false;
    PointCloud pointcloud;

    BinaryWriter writer;
    int framecount;
    public string SaveFileName;
    Thread thread;

    string FolderPath;
    // Use this for initialization
    void Start () {

        this.pointcloud = this.pointCloudShadow.GetComponent<PointCloud>();
        //保存するフォルダ
        //this.writer = new BinaryWriter(File.OpenWrite("C:\\Users\\yamakawa\\Documents\\UnitySave" + @"\"+ "test"));
        this.writer = new BinaryWriter(File.OpenWrite("C:\\Users\\yamakawa\\Documents\\UnitySave" + @"\"+ SaveFileName));
        framecount = 0;
        thread = new Thread(new ThreadStart(Save));
        thread.Start();
        //FolderPath = EditorUtility.OpenFilePanel("ファイル選択", "", "");
    }

    // Update is called once per frame
    void Update () {

        
    }

    //void OnApplicationQuit()
    //{
    //    thread.Abort();
    //}

    void Save()
    {
        while (true)
        {
            
            //Debug.Log(framecount);
            writer.Write(pointcloud.SaveRawData.Length);

            for (int i = 0; i < pointcloud.SaveRawData.Length; i++)
            {

                writer.Write(pointcloud.SaveRawData[i]);
            }

            framecount++;
            if (this.IsSaveStop)
            {
                //thread.Abort();
                break;
            }

        }


    }

    

}
