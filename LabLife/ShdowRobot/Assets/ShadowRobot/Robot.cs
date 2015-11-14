using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Robot
{
    /// <summary>
    /// Not人型 But 光源ロボット
    /// 目標：
    ///     人同士の関係によって動く
    ///     人に影響をうけつつ影響を与える
    /// </summary>
    /// 
    delegate void _eventhandler(List<Human> list_human);

    public class Robot : MonoBehaviour
    {
        #region
        //public GameObject model;
        public GameObject robot;
        public GameObject CIPCforLaserScaner;
        public GameObject CIPCforKinect;
        //モード
        bool IsKinectHuman;
        //CIPC
        List<Vector3> list_humanpos;
        List<Human> List_Human;
        //動き IRobotActionを継承して書く
        CalculatePosition cp;
        //RobotPosition robotPosition;
        //LightAction lightAction;
        #endregion
        //実験用　randonな動き
        Vector3 vec;
        int frame;
        int interval;
        public bool IsRandom;
        // Use this for initialization
        void Start()
        {
            this.List_Human = new List<Human>();
            this.list_humanpos = new List<Vector3>();

            this.vec = Vector3.zero;
            this.frame = 0;
            this.interval = 5;
            
            this.IsKinectHuman = true;

            this.cp = new CalculatePosition(0);

            //this.robotPosition = new RobotPosition();
            //this.lightAction = new LightAction();
            Debug.Log("Light Robot");
        }

        // Update is called once per frame
        void Update()
        {
            if (this.IsRandom) { this.MoveRandom(); }
            else if (this.IsKinectHuman) { this.MoveByKinectHuman(); }
            else { this.MoveByPosition(); }
        }

        void MoveByKinectHuman()
        {
            this.List_Human = this.CIPCforKinect.GetComponent<CIPCReceiver>().List_Humans;
            if (this.List_Human.Count != 0)
            {
                //位置
                Vector3 center = this.cp.CenterPosition(this.List_Human);
                Vector3 vec = center - this.robot.transform.position;
                vec = new Vector3(vec.x, 0, vec.z);
                vec /= vec.magnitude;
                this.robot.transform.position += vec / 100;
                //this.lightAction.Action(this.List_Human, ref this.robot);
            }
        }
        void MoveByPosition()
        {
            this.list_humanpos = this.CIPCforLaserScaner.GetComponent<CIPCReceiverLaserScaner>().list_humanpos;
            if (this.list_humanpos.Count != 0)
            {
                Vector3 center = - this.cp.CenterPosition(this.list_humanpos);
                Vector3 vec = center - this.robot.transform.position;
                vec = new Vector3(vec.x, 0, vec.z);
                vec /= vec.magnitude;
                this.robot.transform.position += vec / 100;
                //this.robot.transform.position += new Vector3(vec.x, 0, vec.z);

            }

        }
        void MoveRandom()
        {
            
            if (this.frame % (this.interval * 60) == 0)
            {
                //位置
                this.vec = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
                //this.interval = Random.Range(0,5);
                this.frame = 1;
            }
            //位置
            this.vec /= (this.vec.magnitude * 100);
            Vector3 pos = this.robot.transform.position + vec;
            if (pos.x > -2f && pos.x < 2f && pos.z > -3 && pos.z < 1.8)
            {
                this.robot.transform.position += vec;
                this.frame++;

            }
            else { this.frame = 0; }
            //this.lightAction.Action(this.List_Human, ref this.robot);

        }
        //パラメータ
        public void ChangeMode(bool Iskinecthuman)
        {
            this.IsKinectHuman = Iskinecthuman;
        }
        public void ChangeVel(bool Is)
        {
            //this.robotPosition.ChangeVel(Is);
        }
        public void ChangeIsHigh(bool Is)
        {
            //this.robotPosition.ChangeIsHigh(Is);
        }
        public void ChangeIsJumpVel(bool Is)
        {
            //this.robotPosition.ChangeIsJumpVel(Is);
        }
        public void ChangeJump(bool Is)
        {
            //this.robotPosition.ChangeJump(Is);
        }
        
    }
}
