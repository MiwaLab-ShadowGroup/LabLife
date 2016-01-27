using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEditor;
using System.Collections.Generic;
using System;

public class SavaData : MonoBehaviour, ISave {

    public enum _Mode{Writer, Reader, }
    public _Mode mode;
    public enum _SaveMode { Byn, CSV, CSVByn,}
    public _SaveMode saveMode;
    public bool SaveSCV = true;
    //
    public GameObject Robot;
    public GameObject RobotLight;
    public GameObject Shadow;
    public GameObject LRF;
    public bool IsRobot;
    public bool IsShadow;
    public bool IsLRF;   
    //
    public bool FolderChoose;
    public string fileName;
    public bool FileChoose;
    public string path;
    public bool IsStart;

    PointCloud pointCloud;
    CIPCReceiverLaserScaner cipcForLRF;

    BinaryWriter bWriter;
    StreamWriter sWtiret;
    BinaryReader bReader;
    StreamReader sReader;

    Thread thread;
    FPSAdjuster.FPSAdjuster FpsAd;
    Vector3 robotPos;
    Vector3 robotLightPos;
    [HideInInspector]
    public ushort[] readData;
    [HideInInspector]
    public List<LRFdataSet> list_Data;
    int frameByn;
    int frameCSV;
    int savemodeflag = 0;

    // Use this for initialization
    void Start ()
    {
        switch (this.mode)
        {
            case _Mode.Reader: this.SetUpReader(); break;
            case _Mode.Writer: this.SetUpWriter(); break;
        }
        this.FpsAd = new FPSAdjuster.FPSAdjuster();
        this.FpsAd.Fps = 30;
        this.FpsAd.Start();
        this.frameByn = 0;
        this.frameCSV = 0;
    }
    void SetUpWriter()
    {
        if (this.Shadow != null)
        {
            this.pointCloud = this.Shadow.GetComponent<PointCloud>();
            this.IsShadow = true;
        }
        if (this.LRF != null)
        {
            this.cipcForLRF = this.LRF.GetComponent<CIPCReceiverLaserScaner>();
            this.IsLRF = true;
        }
        if (this.Robot != null)
        {
            this.IsRobot = true;
        }

        
    }
    void StartSave()
    {
        if (this.IsRobot)  this.savemodeflag += 1;
        if (this.IsLRF)    this.savemodeflag += 10;
        if (this.IsShadow) this.savemodeflag += 100;

        this.bWriter = new BinaryWriter(File.OpenWrite(this.path + @"\" + this.fileName));
        this.sWtiret = new StreamWriter(File.OpenWrite(this.path + @"\" + this.fileName + ".csv"));
    }
    void StopSave()
    {
        if(this.bWriter != null) { this.bWriter.Close(); }
        if(this.sWtiret != null) { this.sWtiret.Close(); }
    }
    void SetUpReader()
    {
        this.readData = new ushort[512 * 424];
    }
    void StartRead() { }

    // Update is called once per frame
    void Update ()
    {
        if(this.mode == _Mode.Writer)
        {
            this.robotPos = this.Robot.transform.position;
            this.robotLightPos = this.RobotLight.transform.position;
        }
        
        //フォルダ選択
        if (this.FolderChoose && this.mode == _Mode.Writer)
        {
            this.path = EditorUtility.SaveFolderPanel("フォルダ選択", " ", " ");
            this.FolderChoose = false;
        }
        if (this.FileChoose && this.mode == _Mode.Reader)
        {
            this.path = EditorUtility.OpenFilePanel("ファイル選択", " ", " ");
            this.FileChoose = false;
        }
        if (this.IsStart && this.thread == null)
        {
            this.StartThread();
        }     
    }

    void StartThread()
    {
        switch (this.mode)
        {
            case _Mode.Reader: this.thread = new Thread(new ThreadStart(this.Read));  break;
            case _Mode.Writer: this.thread = new Thread(new ThreadStart(this.Wtire)); break;
        }
        this.thread.Start();
        Debug.Log("StartThread");
    }

    void Wtire()
    {
        try
        {
            this.StartSave();
            while (true)
            {
                //Debug.Log("OK");
                switch (this.saveMode)
                {
                    case _SaveMode.Byn: this.SaveBiynary(); break;
                    case _SaveMode.CSV: this.SaveCSV(); break;
                    case _SaveMode.CSVByn: this.SaveBiynary(); this.SaveCSV(); break;
                }
                if (!this.IsStart) break;
            }
            this.StopSave();
        } catch { }
    }
    void SaveBiynary()
    {
        this.bWriter.Write(this.frameByn);
        this.bWriter.Write(int.Parse(DateTime.Now.ToString("HHmmssfff")));
        this.bWriter.Write(this.savemodeflag);
        if (this.IsRobot)
        {
            this.bWriter.Write(this.robotPos.x);
            this.bWriter.Write(this.robotPos.y);
            this.bWriter.Write(this.robotPos.z);
            this.bWriter.Write(this.robotLightPos.x);
            this.bWriter.Write(this.robotLightPos.y);
            this.bWriter.Write(this.robotLightPos.z);
        }
        if (this.IsShadow)
        {
            foreach(var p in this.pointCloud.SaveRawData)
            {
                this.bWriter.Write(p);
            }
        }
        if (this.IsLRF)
        {
            this.bWriter.Write(this.cipcForLRF.list_data.Count);
            foreach (var p in this.cipcForLRF.list_data)
            {
                this.bWriter.Write(p.id);
                this.bWriter.Write(p.pos.x);
                this.bWriter.Write(p.pos.y);
                this.bWriter.Write(p.pos.z);
            }
        }
        this.frameByn++;

    }
    void SaveCSV()
    {
        
        List<string> str = new List<string>();
        str.Add(this.frameCSV.ToString());
        str.Add(DateTime.Now.ToString("HHmmssfff"));
        str.Add(this.savemodeflag.ToString());
        if (this.IsRobot)
        {
            str.Add(this.robotPos.x.ToString());
            str.Add(this.robotPos.y.ToString());
            str.Add(this.robotPos.z.ToString());
            str.Add(this.robotLightPos.x.ToString());
            str.Add(this.robotLightPos.y.ToString());
            str.Add(this.robotLightPos.z.ToString());

        }
        if (this.IsLRF)
        {
            str.Add(this.cipcForLRF.list_data.Count.ToString());
            foreach(var p in this.cipcForLRF.list_data)
            {
                str.Add(p.id.ToString());
                str.Add(p.pos.x.ToString());
                str.Add(p.pos.y.ToString());
                str.Add(p.pos.z.ToString());
            }
        }
        string streamData = string.Join(",", str.ToArray());
        this.sWtiret.WriteLine(streamData);
        this.frameCSV++;
    }

    void Read()
    {
        try
        {
            this.StartRead();
            while (true)
            {
                
                this.bReader.ReadInt32();
                this.bReader.ReadInt32();
                int mode = this.bReader.ReadInt32();
                if(mode % 10 == 1)
                {
                    //robot
                    this.robotPos.x = this.bReader.ReadInt32();
                    this.robotPos.y = this.bReader.ReadInt32();
                    this.robotPos.z = this.bReader.ReadInt32();
                    this.robotLightPos.x = this.bReader.ReadInt32();
                    this.robotLightPos.y = this.bReader.ReadInt32();
                    this.robotLightPos.z = this.bReader.ReadInt32();

                }
                if (mode % 10 == 1)
                {
                    //Shadow
                    for (int i = 0; i < 512 * 424; i++)
                    {
                        this.readData[i] = this.bReader.ReadUInt16();

                    }
                }
                if (mode / 100 == 1)
                {
                    //LRF
                    List<LRFdataSet> lsit = new List<LRFdataSet>();
                    for(int i = 0; i < this.bReader.ReadInt32(); i++)
                    {
                        LRFdataSet set = new LRFdataSet();
                        set.id = this.bReader.ReadInt32();
                        set.pos = new Vector3(this.bReader.ReadSingle(), this.bReader.ReadSingle(), this.bReader.ReadSingle());
                        lsit.Add(set);
                    }                
                   
                    this.list_Data = lsit;
                }

              
                if (!this.IsStart)
                {
                    this.StopRead();
                    break;
                }
            }
        }
        catch { }
    }
    void StopRead()
    {
        if(this.bReader != null)
        {
            this.bReader.Close();
        }
        if(this.sReader != null)
        {
            this.sReader.Close(); 
        }
    }

    void OnDestroy()
    {

        if(this.thread != null)
        {
            this.thread.Abort();
        }
        this.StopSave();
        this.StopRead();
    }

    public bool SetIsStart
    {
        set { this.IsStart = value; }

    }
}
