using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System;

public class SaveRobotMT : MonoBehaviour {

    public enum _Mode { Binary, CSV, BinaryCSV, }
    public _Mode mode;

    public GameObject robot;
    public GameObject robotLight;

    public bool FolderChoose;
    public bool IsStart;

    public string fileName;

    BinaryWriter wirter;
    StreamWriter streamWriter;

    string folderPath;
    Vector3 robotPos;
    Vector3 robotLightPos;
    int frameByn;
    int frameCSV;

    // Use this for initialization
    void Start()
    {
        this.frameByn = 0;
        this.frameCSV = 0;
    }

    // Update is called once per frame
    void Update()
    {
        this.robotPos = this.robot.transform.position;
        this.robotLightPos = this.robotLight.transform.position;
        try
        {
            
            //フォルダ選択
            if (this.FolderChoose)
            {
                this.folderPath = EditorUtility.SaveFolderPanel("フォルダ選択", " ", " ");
                this.FolderChoose = false;
            }
            //セーブスタート
            if (this.IsStart && (this.wirter == null || this.streamWriter == null))
            {

                this.StartSave();
            }
            //セーブストップ
            if (!this.IsStart && (this.wirter != null || this.streamWriter != null))
            {
                this.StopSave();
            }

        }
        catch { }


    }

    void StartSave()
    {
        if (this.folderPath != null)
        {
            this.frameByn = 0;
            this.frameCSV = 0;

            switch (this.mode)
            {
                case _Mode.Binary:
                    this.wirter = new BinaryWriter(File.OpenWrite(this.folderPath + @"\" + this.fileName));
                    break;
                case _Mode.CSV:
                    this.streamWriter = new StreamWriter(File.OpenWrite(this.folderPath + @"\" + this.fileName + ".csv"));
                    break;
                case _Mode.BinaryCSV:
                    this.wirter = new BinaryWriter(File.OpenWrite(this.folderPath + @"\" + this.fileName));
                    this.streamWriter = new StreamWriter(File.OpenWrite(this.folderPath + @"\" + this.fileName + ".csv"));
                    break;
            }
        }
        else
        {
            Debug.Log("Please Choose SaveFolder");
        }
    }

    public void save()
    {
        
        switch (this.mode)
        {
            case _Mode.Binary:
                this.SaveBinary();
                break;
            case _Mode.CSV:
                this.SaveCSV();
                break;
            case _Mode.BinaryCSV:
                this.SaveBinary();
                this.SaveCSV();
                break;
        }
    }

    void SaveBinary()
    {

        
        if (this.folderPath != null && this.IsStart)
        {

            this.wirter.Write(this.frameByn);

            this.wirter.Write(this.robotPos.x);
            this.wirter.Write(this.robotPos.y);
            this.wirter.Write(this.robotPos.z);
            this.wirter.Write(this.robotLightPos.x);
            this.wirter.Write(this.robotLightPos.y);
            this.wirter.Write(this.robotLightPos.z);

            this.frameByn++;
        }


    }
    void SaveCSV()
    {

       
        if (this.folderPath != null && this.IsStart)
        {

            //CSV
            List<string> str = new List<string>();
            str.Add(this.frameCSV.ToString());
            str.Add(this.robotPos.x.ToString());
            str.Add(this.robotPos.y.ToString());
            str.Add(this.robotPos.z.ToString());
            str.Add(this.robotLightPos.x.ToString());
            str.Add(this.robotLightPos.y.ToString());
            str.Add(this.robotLightPos.z.ToString());
            string streamData = string.Join(",", str.ToArray());
            this.streamWriter.WriteLine(streamData);

            this.frameCSV++;
        }

    }
    void OnDestroy()
    {
        
        if (this.wirter != null)
        {
            this.wirter.Close();
        }
        if (this.streamWriter != null)
        {
            this.streamWriter.Close();
        }
        
    }
    void StopSave()
    {

        if (this.wirter != null)
        {
            this.wirter.Close();
        }
        if (this.streamWriter != null)
        {
            this.streamWriter.Close();
        }

    }

    public bool SetIsStart
    {
        set { this.IsStart = value; }

    }
}
