using UnityEngine;
using System.Collections;
using System.IO;
using System.Threading;

public class ReadDepth : MonoBehaviour {

    
    BinaryReader reader;
    public ushort[] readData;
    int datalength;
    public string ReadFileName;
    // Use this for initialization
    bool Isreader = true;
    Thread thread;

    void Start () {


        readData = new ushort[512 * 424];

        this.reader = new BinaryReader(File.OpenRead("C:\\Users\\yamakawa\\Documents\\UnitySave" + @"\" + ReadFileName));
        this.thread = new Thread(new ThreadStart(this.ReadData));
        this.thread.Start();
    }
	
	// Update is called once per frame
	void Update () {

        //if (Isreader == true)
        //{

            //this.datalength = this.reader.ReadInt32();

            //for (int i = 0; i < datalength; i++)
            //{
            //    this.readData[i] = this.reader.ReadUInt16();

            //}

            //if (reader.PeekChar() == -1)
            //{
            //    reader.Close();
            //    //Isreader = false;
            //}

            //Debug.Log("OK");
        //}
    }

    void ReadData()
    {
        while (true)
        {
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

                Debug.Log("OK");
            }
            else { break; }
        }


    }

}
