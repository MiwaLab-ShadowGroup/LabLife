using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ViewLRFRecData : MonoBehaviour {

    public GameObject cube;
    public GameObject text;
    public int size;
    public GameObject recorder;

    SavaData data;
    GameObject[] cubes;
    GameObject[] texts;
    List<LRFdataSet> list_Data;

    // Use this for initialization
    void Start () {
        this.data = this.recorder.GetComponent<SavaData>();
        this.cubes = new GameObject[this.size];
        this.texts = new GameObject[this.size];
        for (int i = 0; i < this.size; i++)
        {
            this.cubes[i] = Instantiate(this.cube);
            this.texts[i] = Instantiate(this.text);
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(this.data.list_Data != null && this.data.list_Data.Count > 0)
        {
 
            this.list_Data = this.SortData(this.data.list_Data);
            for (int i = 0; i < this.list_Data.Count; i++)
            {
                if (i < this.size)
                {
                    this.cubes[i].transform.position = this.list_Data[i].pos;
                    this.texts[i].transform.position = this.list_Data[i].pos + new Vector3(0, 0.5f, 0);
                    this.texts[i].GetComponent<TextMesh>().text = this.list_Data[i].id.ToString();
                }
            }
            if (this.size > this.list_Data.Count)
            {
                for (int i = this.list_Data.Count; i < this.size; i++)
                {
                    this.cubes[i].transform.position = new Vector3(0, -100, 0);
                    this.texts[i].transform.position = new Vector3(0, -100, 0);
                }
            }
        }
        
	}

    List<LRFdataSet> SortData(List<LRFdataSet> list)
    {
        List<LRFdataSet> dstList = new List<LRFdataSet>();
        int myID = -1;
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].id == myID)
            {
                dstList[dstList.Count - 1].pos = (dstList[dstList.Count - 1].pos + list[i].pos) / 2;
            }
            else
            {
                dstList.Add(list[i]);
            }
            myID = list[i].id;
        }
        return dstList;
    }

}
