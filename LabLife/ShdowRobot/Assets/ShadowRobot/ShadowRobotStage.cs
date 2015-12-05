using UnityEngine;
using System.Collections;

public class ShadowRobotStage : MonoBehaviour {

    //対象
    public GameObject robot;
    public GameObject FixedLight;

    //コピー
    GameObject localRobot;
    GameObject localFixedLight;

	// Use this for initialization
	void Start ()
    {
        this.localRobot = Instantiate(this.robot);
        this.localFixedLight = Instantiate(this.FixedLight);
        this.localRobot.GetComponent<MeshRenderer>().enabled = true;
        this.localRobot.GetComponentInChildren<MeshRenderer>().enabled = true;
        this.localRobot.transform.parent = this.transform;
        this.localFixedLight.transform.parent = this.transform;
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        //同期する
        this.localRobot.transform.localPosition = this.robot.transform.position;
        this.localFixedLight.transform.localPosition = this.FixedLight.transform.position;
	
	}
}
