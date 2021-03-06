﻿using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEditor;

public class ReadDepth : MonoBehaviour {

    
    BinaryReader reader;
    public ushort[] readData;
    int datalength;
    public string ReadFileName;
    
    bool Isreader = true;
    Thread thread;

    FPSAdjuster.FPSAdjuster FpsAd;

    public bool OpenFileChoose = false;

    string FilePath;

    public bool Readstart; 

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
                this.reader = new BinaryReader(File.OpenRead("C:\\Users\\yamakawa\\Documents\\UnitySave" + @"\" + ReadFileName));
                this.thread = new Thread(new ThreadStart(this.ReadData));
                this.thread.Start();

                this.FpsAd = new FPSAdjuster.FPSAdjuster();
                this.FpsAd.Fps = 30;
                this.FpsAd.Start();
            }

            OpenFileChoose = false;
        }


    }




    void ReadData()
    {
        while (true)
        {
            FpsAd.Adjust();

            if (Isreader == true)
            {

                this.datalength = this.reader.ReadInt32();

                for (int i = 0; i < datalength; i++)
                {
                    this.readData[i] = this.reader.ReadUInt16();

                }

                if (reader.PeekChar() == -1)
                {
                    reader.Close();
                    Isreader = false;
                }

                //Debug.Log("OK");
            }
            else { break; }
        }


    }

}
