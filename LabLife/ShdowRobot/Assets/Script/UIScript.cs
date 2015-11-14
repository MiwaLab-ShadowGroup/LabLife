using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Robot;

public class UIScript : MonoBehaviour
{

    public Camera Projector0;
    public Camera Projector1;
    public Camera Projector2;
    public GameObject Light;
    public List<GameObject> List_Light;
    public GameObject model;
    public GameObject robot;
    public GameObject ModelView;
    public PhysicMaterial RobotPM;
    public GameObject PointCloudShadow;
    public GameObject CIPCforLS;

    int LightID;
    RenderTexture texture;
    Vector3 orgPosLight;
    List<Vector3> List_orgPosLight;
    Vector3 orgPosProjector0;
    Vector3 orgPosProjector1;
    Vector3 orgPosProjector2;
    Quaternion orgQuatProjector0;
    Quaternion orgQuatProjector1;
    Quaternion orgQuatProjector2;

    CIPCReceiver cipcR;
    public InputField myPort;
    public InputField IP;
    public InputField serverPort;


    // Use this for initialization
    void Start()
    {
        this.orgPosLight = this.Light.transform.position;
        this.List_orgPosLight = new List<Vector3>();
        for (int i = 0; i < this.List_Light.Count; i++)
        {
            this.List_orgPosLight.Add(this.List_Light[i].transform.position);
        }
        this.orgPosProjector0 = this.Projector0.transform.position;
        this.orgPosProjector1 = this.Projector1.transform.position;
        this.orgPosProjector2 = this.Projector2.transform.position;
        this.orgQuatProjector0 = this.Projector0.transform.rotation;
        this.orgQuatProjector1 = this.Projector1.transform.rotation;
        this.orgQuatProjector2 = this.Projector2.transform.rotation;
        this.LightID = 0;
    }

    //PointLightVer
    #region
    public void ViewOfLight(bool Is)
    {
        this.Light.SetActiveRecursively(Is);
    }
    public void Changed_LightID(float id)
    {
        this.LightID = (int)id;
    }
    public void Changed_LightPositonX(float x)
    {
        //Vector3 Pos = new Vector3(this.orgPosLight.x + x, this.Light.transform.position.y, this.Light.transform.position.z);
        //this.Light.transform.position = Pos;

        Vector3 prePos = this.List_Light[this.LightID].transform.position;
        Vector3 Pos = new Vector3(x, prePos.y, prePos.z);
        this.List_Light[this.LightID].transform.position = Pos;
    }
    public void Changed_LightPositonY(float y)
    {
        //Vector3 Pos = new Vector3(this.Light.transform.position.x, this.orgPosLight.y + y, this.Light.transform.position.z);
        //this.Light.transform.position = Pos;

        Vector3 prePos = this.List_Light[this.LightID].transform.position;
        Vector3 Pos = new Vector3(prePos.x, y, prePos.z);
        this.List_Light[this.LightID].transform.position = Pos;
    }
    public void Changed_LightPositonZ(float z)
    {
        //Vector3 Pos = new Vector3(this.Light.transform.position.x, this.Light.transform.position.y, this.orgPosLight.z + z);
        //this.Light.transform.position = Pos;

        Vector3 prePos = this.List_Light[this.LightID].transform.position;
        Vector3 Pos = new Vector3(prePos.x, prePos.y, z);
        this.List_Light[this.LightID].transform.position = Pos;
    }
    public void Changed_LightColor_R(float r)
    {
        UnityEngine.Light light = this.List_Light[this.LightID].GetComponent<UnityEngine.Light>();
        Color color = light.color;
        light.color = new Color(r, color.g, color.b, color.a);
    }
    public void Changed_LightColor_G(float g)
    {
        UnityEngine.Light light = this.List_Light[this.LightID].GetComponent<UnityEngine.Light>();
        Color color = light.color;
        light.color = new Color(color.r, g, color.b, color.a);
    }
    public void Changed_LightColor_B(float b)
    {
        UnityEngine.Light light = this.List_Light[this.LightID].GetComponent<UnityEngine.Light>();
        Color color = light.color;
        light.color = new Color(color.r, color.g, b, color.a);
    }
    #endregion
    //DirectLightVer
    #region
    public void Changed_LightRotarionX()
    {
        this.Light.transform.rotation *= Quaternion.AngleAxis(1, Vector3.right);
    }
    public void Changed_LightRotarionY()
    {
        this.Light.transform.rotation *= Quaternion.AngleAxis(1, Vector3.up);
    }
    public void Changed_LightRotarionZ()
    {
        this.Light.transform.rotation *= Quaternion.AngleAxis(1, Vector3.forward);
    }
    public void Changed_LightRotarionX1()
    {
        this.Light.transform.rotation *= Quaternion.AngleAxis(-1, Vector3.right);
    }
    public void Changed_LightRotarionY1()
    {
        this.Light.transform.rotation *= Quaternion.AngleAxis(-1, Vector3.up);
    }
    public void Changed_LightRotarionZ1()
    {
        this.Light.transform.rotation *= Quaternion.AngleAxis(-1, Vector3.forward);
    }
    #endregion
    //Priojector
    #region
    public void Changed_Projector0PositonX(float x)
    {
        Vector3 Pos = new Vector3(this.orgPosProjector0.x + x, this.Projector0.transform.position.y, this.Projector0.transform.position.z);
        this.Projector0.transform.position = Pos;

    }
    public void Changed_Projector0PositonY(float y)
    {
        Vector3 Pos = new Vector3(this.Projector0.transform.position.x, this.orgPosProjector0.y + y, this.Projector0.transform.position.z);
        this.Projector0.transform.position = Pos;

    }
    public void Changed_Projector0PositonZ(float z)
    {
        Vector3 Pos = new Vector3(this.Projector0.transform.position.x, this.Projector0.transform.position.y, this.orgPosProjector0.z + z);
        this.Projector0.transform.position = Pos;

    }
    public void Changed_Projector0Rotarion(float x)
    {

        this.Projector0.transform.rotation = this.orgQuatProjector0 * Quaternion.AngleAxis(x, Vector3.right);

    }
    public void Changed_Projector0Width(float FoV)
    {
        this.Projector0.fieldOfView = FoV;
    }

    public void Changed_Projector1PositonX(float x)
    {
        Vector3 Pos = new Vector3(this.orgPosProjector1.x + x, this.Projector1.transform.position.y, this.Projector1.transform.position.z);
        this.Projector1.transform.position = Pos;

    }
    public void Changed_Projector1PositonY(float y)
    {
        Vector3 Pos = new Vector3(this.Projector1.transform.position.x, this.orgPosProjector1.y + y, this.Projector1.transform.position.z);
        this.Projector1.transform.position = Pos;

    }
    public void Changed_Projector1PositonZ(float z)
    {
        Vector3 Pos = new Vector3(this.Projector1.transform.position.x, this.Projector1.transform.position.y, this.orgPosProjector1.z + z);
        this.Projector1.transform.position = Pos;

    }
    public void Changed_Projector1Rotarion(float x)
    {

        this.Projector1.transform.rotation = this.orgQuatProjector1 * Quaternion.AngleAxis(x, Vector3.right);

    }
    public void Changed_Projector1Width(float FoV)
    {
        this.Projector1.fieldOfView = FoV;
    }

    public void Changed_Projector2PositonX(float x)
    {
        Vector3 Pos = new Vector3(this.orgPosProjector2.x + x, this.Projector2.transform.position.y, this.Projector2.transform.position.z);
        this.Projector2.transform.position = Pos;

    }
    public void Changed_Projector2PositonY(float y)
    {
        Vector3 Pos = new Vector3(this.Projector2.transform.position.x, this.orgPosProjector2.y + y, this.Projector2.transform.position.z);
        this.Projector2.transform.position = Pos;

    }
    public void Changed_Projector2PositonZ(float z)
    {
        Vector3 Pos = new Vector3(this.Projector2.transform.position.x, this.Projector2.transform.position.y, this.orgPosProjector2.z + z);
        this.Projector2.transform.position = Pos;

    }
    public void Changed_Projector2Rotarion(float x)
    {

        this.Projector2.transform.rotation = this.orgQuatProjector2 * Quaternion.AngleAxis(x, Vector3.right);

    }
    public void Changed_Projector2Width(float FoV)
    {
        this.Projector2.fieldOfView = FoV;
    }
    #endregion

    //CIPC
    public void ConnectCIPC()
    {
        this.cipcR = GameObject.FindGameObjectWithTag("CIPC").GetComponent<CIPCReceiver>();
        this.cipcR.ConnectCIPC(int.Parse(this.myPort.text), this.IP.text, int.Parse(this.serverPort.text));

    }
    public void ConnectCIPCforLS()
    {
        this.CIPCforLS.GetComponent<CIPCReceiverLaserScaner>().ConnectCIPC(int.Parse(this.myPort.text), this.IP.text, int.Parse(this.serverPort.text));

    }
    //TestModelView
    public void ModelVies(bool isView)
    {
        this.model.SetActiveRecursively(isView);
    }
    //TestModelView
    public void ShadowVies(bool isView)
    {
        this.ModelView.SetActiveRecursively(isView);
    }
    public void RobotVies(bool isView)
    {
        this.robot.SetActiveRecursively(isView);
    }
    public void ChangeGravity(float gy)
    {
        Physics.gravity = new Vector3(0, -gy, 0);
    }
    
    //Robot
    public void ChangeIsKinectHuman(bool Is)
    {
        this.robot.GetComponent<Robot.Robot>().ChangeMode(Is);
    }
    public void IsChangeVel(bool Is)
    {
        GameObject.FindGameObjectWithTag("Generator").GetComponent<Robot.RobotControll>().ChangeVel(Is);
        this.robot.GetComponent<Robot.Robot>().ChangeVel(Is);
    }
    public void IsChangeHight(bool Is)
    {
        GameObject.FindGameObjectWithTag("Generator").GetComponent<Robot.RobotControll>().ChangeIsHigh(Is);
        this.robot.GetComponent<Robot.Robot>().ChangeIsHigh(Is);
    }
    public void IsChangeJump(bool Is)
    {
        GameObject.FindGameObjectWithTag("Generator").GetComponent<Robot.RobotControll>().ChangeIsJumpVel(Is);
        this.robot.GetComponent<Robot.Robot>().ChangeIsJumpVel(Is);
    }
    public void IsJump(bool Is)
    {
        GameObject.FindGameObjectWithTag("Generator").GetComponent<Robot.RobotControll>().ChangeJump(Is);
        this.robot.GetComponent<Robot.Robot>().ChangeJump(Is);
    }
    public void ChangeBoundness(float b)
    {
        this.RobotPM.bounciness = b;
    }

    //PointCloudShadow
    public void PointCloudShadowIsShow(bool Is)
    {
        this.PointCloudShadow.SetActiveRecursively(Is);
    }
    public void PointCloudRnageXmin(float minx)
    {       
        Vector2 vec = new Vector2 (minx ,this.PointCloudShadow.GetComponent<PointCloud>().rangex.y);
        this.PointCloudShadow.GetComponent<PointCloud>().rangex = vec;
    }
    public void PointCloudRnageXmax(float maxx)
    {
        Vector2 vec = new Vector2( this.PointCloudShadow.GetComponent<PointCloud>().rangex.x, maxx);
        this.PointCloudShadow.GetComponent<PointCloud>().rangex = vec;
    }
    public void PointCloudRnageZmin(float minz)
    {
        Vector2 vec = new Vector2(minz, this.PointCloudShadow.GetComponent<PointCloud>().rangez.y);
        this.PointCloudShadow.GetComponent<PointCloud>().rangez = vec;
    }
    public void PointCloudRnageZmax(float maxz)
    {
        Vector2 vec = new Vector2(this.PointCloudShadow.GetComponent<PointCloud>().rangez.x, maxz);
        this.PointCloudShadow.GetComponent<PointCloud>().rangez = vec;
    }
    public void PointCloudSize(float size)
    {
        this.PointCloudShadow.GetComponent<PointCloud>().size = new Vector3(size,size,size);
    }
    public void PointCloudRange(float size)
    {
        this.PointCloudShadow.GetComponent<PointCloud>().range = (int)size;
    }
    public void KinectPositon(string srt)
    {    
        Vector3 vec = new Vector3(float.Parse(srt.Split(',')[0]),float.Parse(srt.Split(',')[1]),float.Parse(srt.Split(',')[2]));
        this.PointCloudShadow.GetComponent<PointCloud>().ChangeKinectPosition(vec);
    }
}

