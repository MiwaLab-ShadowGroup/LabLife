using UnityEngine;
using System.Collections;
using System.Threading;
using System.IO;
using UnityEditor;

public class Archiverobot : MonoBehaviour
{
    public GameObject robot;
    BinaryReader reader;
    [HideInInspector]

    bool Isreader = true;
    Thread thread;

    FPSAdjuster.FPSAdjuster FpsAd;

    public bool OpenFileChoose = false;

    string FilePath;

    public bool ReadStart = false;

    public bool ReadStop = false;

    bool IsStart = false;

    public bool PausePlay = false;

    public Vector3 robotpos = Vector3.zero;
    int frame;

    // Use this for initialization
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {

        if (OpenFileChoose)
        {
            FilePath = EditorUtility.OpenFilePanel("ファイル選択", "　", "　");

            if (FilePath != null)
            {
                IsStart = true;
                OpenFileChoose = false;
            }
        }

        if (IsStart)
        {
            this.FpsAd = new FPSAdjuster.FPSAdjuster();
            this.FpsAd.Fps = 23;
            this.FpsAd.Start();

            this.reader = new BinaryReader(File.OpenRead(FilePath));
            this.thread = new Thread(new ThreadStart(this.ReadData));
            this.thread.Start();
            IsStart = false;
            Isreader = true;
        }
        
        this.robot.transform.position = this.robotpos;
        

    }

    void ReadData()
    {
        //Debug.Log("ok");
        while (true)
        {
            FpsAd.Adjust();
            if (ReadStart)
            {
                if (Isreader)
                {
                    frame = this.reader.ReadInt32();
                    robotpos.x = this.reader.ReadSingle();
                    robotpos.z = this.reader.ReadSingle();
                    robotpos.y = this.reader.ReadSingle();

                    robotpos.y = 0;

                    if (reader.PeekChar() == -1)
                    {
                        Debug.Log("end");
                        reader.Close();
                        Isreader = false;
                    }

                    if (ReadStop)
                    {
                        Isreader = false;
                        ReadStop = false;
                    }

                }
                else
                {
                    if (reader != null)
                    {
                        reader.Close();

                    }

                    break;
                }

            }
        }

        if (reader != null)
        {
            reader.Close();
        }
        if (FilePath != null)
        {
            FilePath = null;
        }


        ReadStart = false;
        if (thread != null)
        {
            thread.Abort();
        }

    }

    void OnDestroy()
    {
        if (thread != null)
        {
            thread.Abort();

        }
        if (FilePath != null)
        {
            FilePath = null;
        }
        if (reader != null)
        {
            reader.Close();
        }
    }
}
    
    
