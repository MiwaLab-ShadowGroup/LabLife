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


        public MonoBehaviour[] list_robotaction;
        
        public int robotActionID;


        // Use this for initialization
        void Start()
        {
            MonoBehaviour[] array = this.GetComponents<MonoBehaviour>();
            this.list_robotaction = new MonoBehaviour[array.Length - 1];
            for (int i = 0; i < array.Length - 1; i++)
            {
                this.list_robotaction[i] = array[i + 1];
            }

            
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
                }
            }
            
        }

       
    }


}
