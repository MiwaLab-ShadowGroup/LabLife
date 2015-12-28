using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Robot;

public class UIScript : MonoBehaviour
{
    public GameObject CIPCforKinect;
    public GameObject CIPCforLS;
    public GameObject CIPCforRobotSync;
    public GameObject CIPCforRobotSync1;
    public GameObject CIPCforRobotDCP;
    public GameObject SaveCIPC;
    public GameObject SaveButton;
    // Use this for initialization
    void Start()
    {

    }
  
    //CIPC
    public void ConnectCIPC()
    {
        this.CIPCforKinect.GetComponent<CIPCReceiver>().ConnectCIPC();

    }
    public void ConnectCIPCforLS()
    {

        this.CIPCforLS.GetComponent<CIPCReceiverLaserScaner>().ConnectCIPC();

    }
    public void ConnectCIPCforRobotSync()
    {

        this.CIPCforRobotSync.GetComponent<CIPCRobotSync>().ConnectCIPC();
        //this.CIPCforRobotSync1.GetComponent<CIPCRobotSync>().ConnectCIPC();


    }
    public void ConnectCIPCforDCP()
    {

        this.CIPCforRobotSync.GetComponent<CIPCforDCP>().ConnectCIPC();

    }
    public void ConnectCIPCforSave()
    {
        this.SaveCIPC.GetComponent<CIPCReceiverforSave>().ConnectCIPC();
    }

   
    public void SaveDepth()
    {
        //this.SaveButton.GetComponent<SaveDepth>().
    }
}

