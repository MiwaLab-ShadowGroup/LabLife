using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using System.IO;


public class BynaryReader : MonoBehaviour {

    public MonoBehaviour saverobot;
    public bool FolderChoose;
    public string fileName;
    struct _set
    {
        public int id;
        public Vector3 pos;
    }
    string filePath;
    public GameObject cube;
    GameObject[] cubes;
    public GameObject text;
    GameObject[] texts;
    SaveRobotMT srMT;

    [Range(1, 60)]
    public int fps;
    public bool IsStart;
    public bool ReStart;
    List<_set> list_data;

    BinaryReader reader;
    public int size;
    // Use this for initialization
    void Start()
    {

        this.list_data = new List<_set>();

        //this.srMT = this.saverobot.GetComponent<SaveRobotMT>();


        this.cubes = new GameObject[this.size];
        this.texts = new GameObject[this.size];
        for (int i = 0; i < this.size; i++)
        {
            this.cubes[i] = Instantiate(this.cube);
            this.texts[i] = Instantiate(this.text);
        }


    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = this.fps;

        if (this.FolderChoose)
        {
            this.filePath = EditorUtility.OpenFilePanel("ファイル選択", " ", " ");
            this.FolderChoose = false;
            this.reader = new BinaryReader(File.OpenRead(this.filePath));
            this.fileName = this.filePath;
        }
        if (this.ReStart)
        {
            this.IsStart = false;
            this.reader = new BinaryReader(File.OpenRead(this.filePath));
            this.ReStart = false;
            this.fileName = this.filePath;
        }

        if(this.IsStart && this.reader != null)
        {
            this.GetData();
            if (this.list_data.Count > 0)
            {
                for (int i = 0; i < this.list_data.Count; i++)
                {
                    if (i < this.cubes.Length)
                    {
                        this.cubes[i].transform.position = this.list_data[i].pos;
                        this.texts[i].transform.position = this.list_data[i].pos + new Vector3(0, 0.5f, 0);
                        this.texts[i].GetComponent<TextMesh>().text = this.list_data[i].id.ToString();
                    }

                }
                if (this.list_data.Count < this.cubes.Length)
                {
                    for (int i = this.list_data.Count; i < this.cubes.Length; i++)
                    {
                        this.cubes[i].transform.position = new Vector3(0, -100, 0);
                        this.texts[i].transform.position = new Vector3(0, -100, 0);
                    }
                }
            }
        }
         

    }


    void GetData()
    {
        try
        {
            List<_set> list_position = new List<_set>();
            int id = -1;
            this.reader.ReadUInt32();
            this.reader.ReadInt64();

            UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
            dec.Source = this.reader.ReadBytes(this.reader.ReadInt32());
            int humanNum = dec.get_int();

            //データ格納
            for (int i = 0; i < humanNum; i++)
            {
                float x = (float)dec.get_double() / 1000;
                float z = (float)dec.get_double() / 1000;
                int myID = dec.get_int();


                Vector3 vec = new Vector3(x, 0, z);

                if (id == myID)
                {
                    vec = (vec + list_position[list_position.Count - 1].pos) / 2;
                    _set set = new _set();
                    set.id = myID;
                    set.pos = vec;
                    list_position[list_position.Count - 1] = set;
                }
                else
                {
                    _set set = new _set();
                    set.id = myID;
                    set.pos = vec;
                    list_position.Add(set);

                }


                id = myID;
            }
            this.list_data = list_position;
        }
        catch
        {
            this.IsStart = false;
        }
        
       

    }
    void OnDestroy()
    {
        this.reader.Close();
    }

    
}
