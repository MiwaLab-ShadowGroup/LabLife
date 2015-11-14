using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelScript : MonoBehaviour {

    public List<GameObject> bones;
    public GameObject joint;
    public GameObject bone;

    List<GameObject> list_joints;
    List<GameObject> list_bone;
    int[] tree;
    //Generator genarator;
    //public Human modeldata { get; set; }
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
	// Use this for initialization
	void Start () 
    {
        //this.genarator = GameObject.FindGameObjectWithTag("Generator").GetComponent<Generator>();	 
        this.MakeJoint(25, ref this.tree);
	}
	
	//Update is called once per frame
    //human情報そのまま適応
	public void move (Human human) 
    {
        try
        {
            //this.transform.position = this.genarator.robotBone.bones[0].position;
            this.bones[0].transform.position = human.bones[0].position;
            //影を出すよう
            this.moveBone(human);

            //当たり判定
            this.Joint(human);
            this.Bone(human);
        }
        catch
        {
            Debug.Log("Erorr:ModelScript");
        }	
	}
    public void moveAndTransToTargetPosition(Human human, int boneNum, Vector3 vec, bool IsSamePose)
    {
        try
        {
            //this.transform.position = this.genarator.robotBone.bones[0].position;
            this.bones[0].transform.position = human.bones[0].position;
            if(IsSamePose) this.moveBone(human);
            this.MovechildBone(bones[boneNum], vec);
            
        }
        catch
        {
            Debug.Log("Erorr:ModelScript");
        }

    }
    public void moveByBonePositon(Human human)
    {
        try
        {
            //this.transform.position = this.genarator.robotBone.bones[0].position;
            this.bones[0].transform.position = human.bones[0].position;

            for (int i = 0; i < this.bones.Count; i++)
            {
                this.bones[i].transform.position = human.bones[i].position;
            }
        }
        catch
        {
            Debug.Log("Erorr:ModelScript");
        }
    }

    //位置の操作
    public void move(Human human, Vector3 vec)
    { 
        try
        {
            //this.transform.position = this.genarator.robotBone.bones[0].position;
            this.bones[0].transform.position = vec;

            this.moveBone(human);
        }
        catch
        {
            Debug.Log("Erorr:ModelScript");
        }

    }
    //位置と向きの操作
    public void move(Human human, Vector3 vec, Quaternion quat)
    {
        
        try
        {
            
            //this.transform.position = this.genarator.robotBone.bones[0].position;
            this.bones[0].transform.position = vec;
            
            for (int i = 1; i < this.bones.Count; i++)
            {
                if (i != 12 && i != 16 && i != 15 && i != 19)
                {
                    //this.bones[i].transform.rotation= this.genarator.robotBone.bones[i].quaternion;
                    this.bones[i].transform.rotation = human.bones[i].quaternion * Quaternion.AngleAxis(180, Vector3.up);
                }
                if (i == 14)
                {
                    this.bones[i].transform.rotation *=  Quaternion.AngleAxis(180, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.up);
                }
                if (i == 18)
                {
                    this.bones[i].transform.rotation *= Quaternion.AngleAxis(180, Vector3.up) * Quaternion.AngleAxis(90, Vector3.up);
                }
            }

            
            //this.bones[0].transform.rotation = quat;
            //this.transform.rotation = quat;
            

            
        }
        catch
        {
            Debug.Log("Erorr:ModelScript");
        }

    }
    //位置と向きのみ操作
    public void move(Vector3 vec, Quaternion quat)
    {
        try
        {
            //this.transform.position = this.genarator.robotBone.bones[0].position;
            this.bones[0].transform.position = vec;
            this.bones[0].transform.rotation = quat;
            this.transform.rotation = quat;
        }
        catch
        {
            Debug.Log("Erorr:ModelScript");
        }
    }    

    void moveBone(Human human)
    {
        for (int i = 0; i < this.bones.Count; i++)
        {
            if (i != 12 && i != 16 && i != 15 && i != 19)
            {
                //this.bones[i].transform.rotation= this.genarator.robotBone.bones[i].quaternion;
                this.bones[i].transform.rotation = human.bones[i].quaternion;
            }
            if (i == 14)
            {
                this.bones[i].transform.rotation *= Quaternion.AngleAxis(-90, Vector3.up);
            }
            if (i == 18)
            {
                this.bones[i].transform.rotation *= Quaternion.AngleAxis(90, Vector3.up);
            }
        }          
    }
    public void moveByMakedQuat(Human human)
    {
        try
        {
            //this.transform.position = this.genarator.robotBone.bones[0].position;
            this.bones[0].transform.position = human.bones[0].position;

            for (int i = 0; i < this.bones.Count; i++)
            {
                if (i != 12 && i != 16 && i != 15 && i != 19)
                {
                    //this.bones[i].transform.rotation= this.genarator.robotBone.bones[i].quaternion;
                    this.bones[i].transform.rotation = human.bones[i].quaternion;
                }
                
            }          
        }
        catch
        {
            Debug.Log("Erorr:ModelScript");
        }

    }

    //手の先の移動距離から全身の移動
    //姿勢は維持
    void MovechildBone(GameObject bone, Vector3 vec)
    {
        bone.transform.position = vec;
        if (bone.transform.parent != null)
        {
            this.MovechildBone(bone.transform.parent.gameObject, vec);
        }
    }
    
    public void ChangeSize(int boneNum, Vector3 size)
    {
        if(boneNum != 12 && boneNum != 16)
        this.bones[boneNum].transform.localScale = size;
    }
    
    //あたり判定用
    void Joint(Human human)
    {
        try
        {
            for (int i = 0; i < this.list_joints.Count; i++)
            {
                this.list_joints[i].transform.position = human.bones[i].position;
            }
        }
        catch{}
    }
    void Bone(Human human)
    {
        try
        {
            for (int i = 0; i < this.list_bone.Count; i++)
            {
                this.list_bone[i].transform.rotation = human.bones[i].quaternion;
                this.list_bone[i].transform.position = (human.bones[i].position + human.bones[this.tree[i]].position) / 2;
                float length = (human.bones[i].position - human.bones[this.tree[i]].position).magnitude;
                this.list_bone[i].transform.localScale = new Vector3(0.1f, length, 0.1f);
            }
        }
        catch { }
    }

    void MakeJoint(int numofJ, ref int[] treeofBone)
    {
        this.list_joints = new List<GameObject>();
        this.list_bone = new List<GameObject>();

        for (int i = 0; i < numofJ; i++)
        {
            this.list_joints.Add(Instantiate(this.joint));
            this.list_joints[i].transform.parent = this.transform;
        }
        for (int i = 0; i < numofJ - 1 ; i++)
        {
            this.list_bone.Add(Instantiate(this.bone));
            this.list_bone[i].transform.parent = this.transform;
        }
        treeofBone = new int[] {0,0,20,2,20,4,5,6,20,8,9,10,0,12,13,14,0,16,17,18,1,6,7,10,11};
    }

}