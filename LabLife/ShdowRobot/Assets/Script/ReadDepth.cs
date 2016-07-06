﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEditor;

public class ReadDepth : MonoBehaviour {


    //public enum _Mode { MakeArchive, ReadArchive, }
    //public _Mode mode;
    BinaryReader reader;
    [HideInInspector]
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

    public bool PausePlay = false;
    string time;

    // Use this for initialization
    void Start () {

        readData = new ushort[512 * 424];
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
            this.FpsAd = new FPSAdjuster.FPSAdjuster();
            this.FpsAd.Fps = 30;
            this.FpsAd.Start();

            this.reader = new BinaryReader(File.OpenRead(FilePath));
            this.thread = new Thread(new ThreadStart(this.ReadData));
            this.thread.Start();
            IsStart = false;
            Isreader = true;
            //Debug.Log("isstart");
        }
    }

    void ReadData()
    {
        //Debug.Log("ok");
        while (true)
        {
            if (!PausePlay)
            {
                FpsAd.Adjust();
                if (ReadStart)
                {
                    if (Isreader)
                    {
                        //Debug.Log("ok2");
                        this.time = reader.ReadString();

                        this.datalength = this.reader.ReadInt32();
                        Debug.Log(time);


                        for (int i = 0; i < datalength; i++)
                        {
                            this.readData[i] = this.reader.ReadUInt16();

                        }

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
                        //Debug.Log("OK");

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
        }
        //Debug.Log("thread");

        if (reader != null)
        {
            reader.Close();
        }
        if (FilePath != null)
        {
            FilePath = null;
        }

        //readData = new ushort[512 * 424];

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
