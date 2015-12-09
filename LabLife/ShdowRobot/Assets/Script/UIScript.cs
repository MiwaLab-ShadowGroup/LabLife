using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Robot;

public class UIScript : MonoBehaviour
{
    public GameObject CIPCforKinect;
    public GameObject CIPCforLS;

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

}

