using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEditor;


public class SaveDepth : MonoBehaviour {

    public GameObject pointCloudShadow;
    
    PointCloud pointcloud;

    BinaryWriter writer;

    Thread thread;

    string FolderPath;

    FPSAdjuster.FPSAdjuster FpsAd;

    public bool OpenFileChoose = false;

    public bool Savestart = false;

    public bool IsSaveStop = false;

    public string filename;

    // Use this for initialization
    void Start () {

        this.pointcloud = pointCloudShadow.GetComponent<PointCloud>();


        this.FpsAd = new FPSAdjuster.FPSAdjuster();
        this.FpsAd.Fps = 30;
        this.FpsAd.Start();
        //保存するフォルダ

    }
    
    // Update is called once per frame
    void Update () {

        if (OpenFileChoose)
        {
            FolderPath = EditorUtility.SaveFolderPanel("フォルダ選択", " ", " ");

            OpenFileChoose = false;

        }
        if (FolderPath != null)
            {
                if (Savestart)
                {
                    this.writer = new BinaryWriter(File.OpenWrite(FolderPath + @"\" + filename));
                    thread = new Thread(new ThreadStart(Save));
                    thread.Start();
                    
                    
                    Savestart = false;
                   
                }

            }

    }


    void OnApplicationQuit()
    {
        if(thread != null)
        {
            thread.Abort();
            
        }
        if (FolderPath != null)
        {
            FolderPath = null;
        }
    }

    void Save()
    {
        try
        {
            while (true)
            {
                this.FpsAd.Adjust();
                //Debug.Log(framecount);
                writer.Write(pointcloud.SaveRawData.Length);

                for (int i = 0; i < pointcloud.SaveRawData.Length; i++)
                {

                    writer.Write(pointcloud.SaveRawData[i]);
                }

                //framecount++;
                if (this.IsSaveStop)
                {

                    break;

                }

            }

            writer.Close();

        }
        catch
        {

        }
    }

}
