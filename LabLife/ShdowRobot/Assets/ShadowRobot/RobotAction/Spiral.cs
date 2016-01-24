using UnityEngine;
using System.Collections;

public class Spiral : MonoBehaviour {

    public GameObject robot;
    GameObject robotLight;
    public bool IsHeight;
    public bool OnlySin = false;
    public float angularVelocity;
    public float Rvel;
    public float radian;
    public float MaxRadius;
    public Vector3 target;
    Vector3 robotPos;
   public float radius;

	// Use this for initialization
	void Start()
    {
        this.radius = 0;
        this.target = Vector3.right;
        this.robotPos = Vector3.zero;
        this.robotLight = this.robot.transform.FindChild("RobotLight").gameObject;

    }

    // Update is called once per frame
    void Update ()
    {
        try
        {
            if (this.OnlySin)
            {
                if (this.IsHeight)
                {
                    float y = 2f + Mathf.Sin(this.angularVelocity * Time.fixedTime);
                    Vector3 lightVec = new Vector3(0, y - this.robotLight.transform.position.y, 0);

                    this.robotLight.transform.position += lightVec;
                }
            }
            else
            {
                this.radius += (this.Rvel * Time.deltaTime);

                if (this.radius > this.MaxRadius || this.radius < 0)
                {
                    this.Rvel *= -1;
                    if (this.radius > this.MaxRadius) this.radius = this.MaxRadius;
                    if (this.radius < 0) this.radius = (this.Rvel * Time.deltaTime);
                }

                this.robotPos = this.robot.transform.position;
                this.robotPos.y = 0;

                //円中心からの方向ベクトル
                if (this.radius != 0)
                {
                    this.target = (Quaternion.AngleAxis(this.radian * Time.deltaTime, Vector3.up) * (this.target / this.target.magnitude * this.radius));
                    this.target.y = this.robot.transform.position.y;
                    this.robot.transform.position = this.target;
                }
                if (this.target.x == 0 && this.target.z == 0) this.target = Vector3.right;



                if (this.IsHeight)
                {
                    float y = 2f + Mathf.Sin(this.angularVelocity * Time.fixedTime);
                    Vector3 lightVec = new Vector3(0, y - this.robotLight.transform.position.y, 0);

                    this.robotLight.transform.position += lightVec;
                }
            }
            
        }
        catch { }
	
	}
}
