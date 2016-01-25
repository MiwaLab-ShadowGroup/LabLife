using UnityEngine;
using System.Collections;

public class RobotSync : MonoBehaviour
 {

    public GameObject robot;
    public GameObject robotLight;
    public GameObject CIPCforRobotSync;
    public CIPCRobotSync cipc;

    //Use this for initialization
    void Start ()
    {
        this.cipc = this.CIPCforRobotSync.GetComponent<CIPCRobotSync>();
        this.robotLight = GameObject.FindWithTag("RobotLight");
        
    }

    //Update is called once per frame
    void Update()
    {

        this.robot.transform.position = this.cipc.robotPos;
        this.robotLight.transform.position = this.cipc.robotLightPos;
        
    }
}
