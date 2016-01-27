using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;
using UnityEditor;

public class ReadDepthmulti : MonoBehaviour
{

    //public enum _Mode { MakeArchive, ReadArchive, }
    //public _Mode mode;

    public struct _numberofdata
    {
        public BinaryReader reader;
        public ushort[] readData;
        public string FilePath;
    }
    public int number;

    int datalength;
    Thread thread;

    //BinaryReader reader;
    //[HideInInspector]
    //public ushort[] readData;

    //public string ReadFileName;

    public _numberofdata[] numberofdata;

    bool Isreader = true;

    FPSAdjuster.FPSAdjuster FpsAd;

    public bool OpenFileChoose = false;

    public bool ReadStart = false;

    public bool ReadStop = false;

    bool IsStart = false;

    public bool PausePlay = false;

    // Use this for initialization
    void Start()
    {
        if(numberofdata.Length == 0)
        {
            return;
        }
        for (int i = 0; i < numberofdata.Length; i++)
        {
            numberofdata[i].readData = new ushort[512 * 424];
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OpenFileChoose)
        {
            for(int i = 0; i < numberofdata.Length; i++)
            {
                numberofdata[i].FilePath = EditorUtility.OpenFilePanel("ファイル選択", "　", "　");
            }
            

            if (numberofdata[0].FilePath != null)
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

            for(int i = 0; i < numberofdata.Length; i++)
            {
                this.numberofdata[i].reader = new BinaryReader(File.OpenRead(numberofdata[i].FilePath));
                this.thread = new Thread(new ThreadStart(this.ReadData));
                this.thread.Start();
                //this.numberofdata[i].number = i;
            }
            
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
                        for (int j = 0; j < numberofdata.Length; j++)
                        {
                            this.datalength = this.numberofdata[j].reader.ReadInt32();
                            for (int i = 0; i < datalength; i++)
                            {
                                this.numberofdata[j].readData[i] = this.numberofdata[j].reader.ReadUInt16();

                            }
                            if (numberofdata[j].reader.PeekChar() == -1)
                            {
                                //Debug.Log("end");
                                numberofdata[j].reader.Close();
                                Isreader = false;
                            }

                            if (ReadStop)
                            {
                                Isreader = false;
                                ReadStop = false;
                            }

                        }
                        //Debug.Log("OK");

                    }
                    else
                    {
                        for(int i = 0; i < numberofdata.Length; i++)
                        {
                            if (numberofdata[i].reader != null)
                            {
                                numberofdata[i].reader.Close();

                            }
                        }
                        break;
                    }

                }
            }
        }
        //Debug.Log("thread");
        for(int k = 0; k < numberofdata.Length; k++)
        {
            if (numberofdata[k].reader != null)
            {
                numberofdata[k].reader.Close();
            }
            if (numberofdata[k].FilePath != null)
            {
                numberofdata[k].FilePath = null;
            }
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
        for (int k = 0; k < numberofdata.Length; k++)
        {
            if (numberofdata[k].reader != null)
            {
                numberofdata[k].reader.Close();
            }
            if (numberofdata[k].FilePath != null)
            {
                numberofdata[k].FilePath = null;
            }
        }
    }
}
