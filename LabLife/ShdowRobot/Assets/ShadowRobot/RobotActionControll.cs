using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Robot
{
    /// <summary>
    /// スクリプト管理
    /// </summary>
    public class RobotActionControll : MonoBehaviour
    {

        public GameObject robot;
        public MonoBehaviour[] list_robotaction;
        
        public int robotActionID;
        private Vector3 preRobotPos;
        private Vector3 preLightPos;
        // Use this for initialization
        void Start()
        {
            MonoBehaviour[] array = this.GetComponents<MonoBehaviour>();
            this.list_robotaction = new MonoBehaviour[array.Length - 1];
            for (int i = 0; i < array.Length - 1; i++)
            {
                this.list_robotaction[i] = array[i + 1];
            }
            this.preRobotPos = this.robot.transform.position;
            this.preLightPos = this.robot.transform.FindChild("RobotLight").transform.position;
            
        }

        // Update is called once per frame
        void Update()
        {
            if (this.robotActionID < this.list_robotaction.Length)
            {
                if (!this.list_robotaction[this.robotActionID].enabled)
                {
                    for (int i = 0; i < this.list_robotaction.Length; i++)
                    {
                        if (this.list_robotaction[i].enabled) this.list_robotaction[i].enabled = false;
                    }
                        this.list_robotaction[this.robotActionID].enabled = true;
                    this.Init();
                }
            }
            
        }

        void Init()
        {
            this.robot.transform.position = this.preRobotPos;
            this.robot.transform.FindChild("RobotLight").transform.position = this.preLightPos;

        }
       
    }


}
