using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System;

public class SaveRobotMove : MonoBehaviour, ISave
    {

    public enum _Mode { Binary, CSV, BinaryCSV, }
    public _Mode mode;

    public GameObject robot;

    public bool FolderChoose;
    public bool IsStart;
    
    public string fileName;

    GameObject robotLight;
    BinaryWriter wirter;
    StreamWriter streamWriter;
    System.Diagnostics.Stopwatch stopWatch;
    Thread threadByn;
    Thread threadCsv;
    FPSAdjuster.FPSAdjuster adjuster;
    string folderPath;
    Vector3 robotPos;
    Vector3 robotLightPos;

	// Use this for initialization
	void Start ()
    {
        this.robotLight = this.robot.transform.FindChild("RobotLight").gameObject;     
	}
	
	// Update is called once per frame
	void Update ()
    {
        try
        {
            this.robotPos = this.robot.transform.position;
            this.robotLightPos = this.robotLight.transform.position;

            //フォルダ選択
            if (this.FolderChoose)
            {
                this.folderPath = EditorUtility.SaveFolderPanel("フォルダ選択", " ", " ");
                this.FolderChoose = false;
            }
            //セーブスタート
            if (this.IsStart && this.threadByn == null && this.threadCsv == null)
            {
                this.StartThread();
            }
        }
        catch { }
        
    
	}

    void StartThread()
    {
        if(this.folderPath != null)
        {
            switch (this.mode)
            {
                case _Mode.Binary:
                    this.threadByn = new Thread(new ThreadStart(this.SaveBinary));
                    this.threadByn.Start();
                    break;
                case _Mode.CSV:
                    this.threadCsv = new Thread(new ThreadStart(this.SaveCSV)) ;
                    this.threadCsv.Start();
                    break;
                case _Mode.BinaryCSV:
                    this.threadByn = new Thread(new ThreadStart(this.SaveCSV));
                    this.threadCsv = new Thread(new ThreadStart(this.SaveBinary));
                    this.threadByn.Start();
                    this.threadCsv.Start();
                    break;
            }

            this.stopWatch = new System.Diagnostics.Stopwatch();
            this.stopWatch.Start();
            this.adjuster = new FPSAdjuster.FPSAdjuster();
            this.adjuster.Fps = 30;
            this.adjuster.Start();
        }
        else
        {
            Debug.Log("Please Choose SaveFolder");
        }
    }
    void SaveBinary()
    {
        
        int frame = 0;
        try
        {
            if(this.folderPath != null && this.IsStart)
            {
                this.wirter = new BinaryWriter(File.OpenWrite(this.folderPath + @"\" + this.fileName));

                while (true)
                {
                    this.adjuster.Adjust();

                    this.wirter.Write(frame);
                    this.wirter.Write(this.stopWatch.ElapsedMilliseconds);

                    this.wirter.Write(this.robotPos.x);
                    this.wirter.Write(this.robotPos.y);
                    this.wirter.Write(this.robotPos.z);
                    this.wirter.Write(this.robotLightPos.x);
                    this.wirter.Write(this.robotLightPos.y);
                    this.wirter.Write(this.robotLightPos.z);

                    frame++;
                    if (!this.IsStart) break;
                }

            }

            this.wirter.Close();
        }
        catch { }
    }
    void SaveCSV()
    {

        int frame = 0;
        try
        {
            if (this.folderPath != null && this.IsStart)
            {
                this.streamWriter = new StreamWriter(File.OpenWrite(this.folderPath + @"\" + this.fileName +".csv"));

                while (true)
                {
                    this.adjuster.Adjust();

                    //CSV
                    List<string> str = new List<string>();
                    str.Add(frame.ToString());
                    str.Add(this.stopWatch.ElapsedMilliseconds.ToString());
                    str.Add(this.robotPos.x.ToString());
                    str.Add(this.robotPos.y.ToString());
                    str.Add(this.robotPos.z.ToString());
                    str.Add(this.robotLightPos.x.ToString());
                    str.Add(this.robotLightPos.y.ToString());
                    str.Add(this.robotLightPos.z.ToString());
                    string streamData = string.Join(",", str.ToArray());
                    this.streamWriter.WriteLine(streamData);

                    frame++;
                    if (!this.IsStart) break;
                }

            }

            this.streamWriter.Close();
            
        }
        catch { }
    }
    void OnDestroy()
    {
        if(this.threadByn != null)
        {
            this.threadByn.Abort();
            
        }
        if(this.threadCsv != null)
        {
            this.threadCsv.Abort();
        }
        if(this.wirter != null)
        {
            this.wirter.Close();
        }
        if(this.streamWriter != null)
        {
            this.streamWriter.Close();
        }
        if(this.stopWatch != null)
        {
            this.stopWatch.Stop();
        }
    }

    public bool SetIsStart
    {
      set { this.IsStart = value; }

    }
}
