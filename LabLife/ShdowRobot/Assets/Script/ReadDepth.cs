using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEditor;

public class ReadDepth : MonoBehaviour {

    
    BinaryReader reader;
    public ushort[] readData;
    int datalength;
    //public string ReadFileName;
    
    bool Isreader = true;
    Thread thread;

    FPSAdjuster.FPSAdjuster FpsAd;

    public bool OpenFileChoose = false;

    string FilePath;

    public bool ReadStart = false;

    public bool ReadStop = false;

    bool IsStart = false;

    // Use this for initialization
    void Start () {

        readData = new ushort[512 * 424];
        this.FpsAd = new FPSAdjuster.FPSAdjuster();
        this.FpsAd.Fps = 30;
        this.FpsAd.Start();
    }

    // Update is called once per frame
    void Update()
    {

        if (OpenFileChoose)
        {
            FilePath = EditorUtility.OpenFilePanel("ファイル選択", "　", "　");

            if(FilePath != null)
            {
                IsStart = true;
                OpenFileChoose = false;

            }

        }

        if (IsStart)
        {
            this.reader = new BinaryReader(File.OpenRead(FilePath));
            this.thread = new Thread(new ThreadStart(this.ReadData));
            this.thread.Start();
            IsStart = false;
            //Debug.Log("isstart");
        }
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
                    //Debug.Log("ok2");
                    this.datalength = this.reader.ReadInt32();

                    for (int i = 0; i < datalength; i++)
                    {
                        this.readData[i] = this.reader.ReadUInt16();
                        //Debug.Log("aaaaaa");
                    }

                    if (reader.PeekChar() == -1)
                    {
                        //Debug.Log("ok3");
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
                    Debug.Log("OK");

                    reader.Close();

                    break;
                }

            }

        }

    }

    void OnApplicationQuit()
    {
        if (thread != null)
        {
            thread.Abort();

        }
        if (FilePath != null)
        {
            FilePath = null;
        }
        if(reader!= null)
        {
            reader.Close();
        }
    }
}
