using UnityEngine;
using System.Collections;

public class RandomMove : MonoBehaviour {

    public GameObject robot;
    GameObject robotLight;


    Vector3 vec;
    Vector3 veclight;
    int interval;
    public int velparameter;
    //public Vector4 fieldSize;
    public Vector2 Field;
    public Vector2 localO;
    public bool IsHeigh;
    public bool IsLight;
    
                                   
    Vector2 rangex;
    Vector2 rangez;
    float pretime;
    // Use this for initialization
    void Start ()
    {
        this.Chenge();
        this.interval = 5;

        this.rangex = new Vector2(-this.localO.x, this.Field.x - this.localO.x);
        this.rangez = new Vector2( - this.Field.y + this.localO.y, this.localO.y);

        this.veclight = Vector3.zero;
        this.pretime = 0 ;

        this.robotLight = this.robot.transform.FindChild("RobotLight").gameObject;
        
    }
	
	// Update is called once per frame
	void Update ()
    {
        float time = Time.realtimeSinceStartup;
        if ((int)time % this.interval == 0 && (time - this.pretime) > 1f)
        {
            this.pretime = time;
            this.Chenge();
            this.interval = Random.Range(3, 10);

            //Debug.Log("Change + " + time);
        }
        if (!this.IsHeigh) this.vec.y = 0;
        if (!this.IsLight) this.veclight = Vector3.zero;

        //ロボットの動き
        Vector3 pos = this.robot.transform.position + this.vec;
        if (pos.x > this.rangex.x && pos.x < this.rangex.y && pos.z > this.rangez.x && pos.z < this.rangez.y)
        {
            this.robot.transform.position += this.vec;
        }
        else
        {
            this.Chenge();
            //Debug.Log("change" + pos);
        }

        //光源の動き
        if (this.IsLight)
        {
            Vector3 lightPos = this.robotLight.transform.position + this.veclight;
            if (lightPos.y > 0.5f && lightPos.y < 3)
                this.robotLight.transform.position += this.veclight;
            else this.Chenge();
                
        }
        
    }

    void Chenge()
    {
        this.vec = new Vector3(Random.Range(-50, 50), Random.Range(-50, 50), Random.Range(-50, 50));
        this.vec /= (this.vec.magnitude * this.velparameter);
        this.veclight.y = this.vec.y;
    }
}
