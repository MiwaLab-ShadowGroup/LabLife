using UnityEngine;
using System.Collections;

public class ActionTest : MonoBehaviour {

    //public
    public GameObject robto;
    public GameObject pointCloudShadow;
    public GameObject CIPCforLS;
    PointCloud pointCloud;


    CIPCReceiverLaserScaner cipcLS;
    CalculatePosition CP;

	// Use this for initialization
	void Start ()
    { 
        this.pointCloud = this.pointCloudShadow.GetComponent<PointCloud>();
        this.cipcLS = this.CIPCforLS.GetComponent<CIPCReceiverLaserScaner>();
        this.CP = new CalculatePosition();
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 center = this.pointCloud.centerPos;

        	
	}
}
