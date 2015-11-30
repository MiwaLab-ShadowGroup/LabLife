using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Robot;

public class UIScript : MonoBehaviour
{
    public GameObject CIPCforKinect;
    public GameObject CIPCforLS;
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

    public void ConnectCIPCforSave()
    {
        this.SaveCIPC.GetComponent<CIPCReceiverforSave>().ConnectCIPC();
    }

   
    public void SaveDepth()
    {
        //this.SaveButton.GetComponent<SaveDepth>().
    }
}

