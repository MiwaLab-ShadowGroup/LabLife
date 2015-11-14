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
    public class RobotControll : MonoBehaviour
    {

        //public GameObject model;
        public GameObject robot;

        List<Human> List_Human;
        List<Human> List_Human_Last;
        CIPCReceiver cipc;
        //モード
        bool IsChangeVelocity;
        bool IsHight;
        bool IsJumpVel;
        bool IsJump;
        enum _MoveMode
        {
            center, craw,
        }
        _MoveMode movemode;
        //Jump用
        Vector3 preVel;
        Vector3 prePos;

        // Use this for initialization
        void Start()
        {
            this.List_Human = new List<Human>();
            this.List_Human_Last = new List<Human>();
            this.cipc = GameObject.FindGameObjectWithTag("CIPC").GetComponent<CIPCReceiver>();
            this.IsChangeVelocity = false;
            this.IsHight = true;
            this.IsJumpVel = false;
            this.IsJump = false;

            this.preVel = Vector3.zero;
            this.prePos = Vector3.zero;

            Debug.Log("Light Robot");
        }

        // Update is called once per frame
        void Update()
        {

            this.List_Human_Last = this.List_Human;
            this.List_Human = this.cipc.List_Humans;
            if (this.List_Human.Count != 0)
            {
                this.Move(this.List_Human);
                if (this.IsJump)
                    this.Jump(this.List_Human);
            }
        }

        //Action
        //xz平面上移動
        void Move(List<Human> list_human)
        {
            Vector3 centerVec = this.CenterPosition(list_human);//+ new Vector3(0, 0, 0.5f);
            Vector3 vec = centerVec - this.robot.transform.position;
            vec = vec / vec.magnitude;

            if (this.IsChangeVelocity)
            {
                float vel = this.Velocity().magnitude;
                this.robot.transform.position += vec / vel / 1000;
            }
            else
            {
                //this.robot.GetComponent<Rigidbody>().AddForce(vec, ForceMode.Acceleration);
                this.robot.transform.position += vec / 100;
            }

        }

        //上に飛ぶ
        void Jump(List<Human> list_human)
        {
            int human_Num = list_human.Count;
            Vector3 pos = new Vector3();
            for (int i = 0; i < list_human.Count; i++)
            {
                pos += list_human[i].bones[0].position;
            }
            pos = new Vector3(0, pos.y / human_Num, 0);

            if (this.prePos != Vector3.zero)
            {
                Vector3 vel = (pos - this.prePos) / Time.deltaTime;
                Vector3 acc = (vel - this.preVel) / Time.deltaTime;

                if (this.IsJumpVel)
                {
                    //速度を与える
                    this.robot.GetComponent<Rigidbody>().velocity = vel;
                }
                else
                {
                    //加速度を与える
                    this.robot.GetComponent<Rigidbody>().AddForce(acc * this.robot.GetComponent<Rigidbody>().mass, ForceMode.Force);
                }

                //Debug.Log("JUMP:" + acc.ToString());
                this.prePos = pos;
                this.preVel = vel;
            }
            else
            {
                this.prePos = pos;
            }
        }
        //間に割って入る
        Vector3 Warikomi(List<Human> list_human)
        {
            Vector3 vector = new Vector3();

            float[] length = new float[list_human.Count];
            for (int i = 0; i < length.Length; i++)
            {
                float localLength = 100f;
                for (int j = 0; j < length.Length; j++)
                {
                    if (i != j)
                    {
                        if (localLength > (list_human[j].bones[0].position - list_human[i].bones[0].position).magnitude)
                            localLength = (list_human[j].bones[0].position - list_human[i].bones[0].position).magnitude;
                    }
                }
                length[i] = localLength;
            }

            return vector;
        }
        //よける
        Vector3 Escape(List<Human> list_human, Human robot)
        {
            Vector3 vector = new Vector3();
            float[] length = new float[list_human.Count];
            for (int i = 0; i < list_human.Count; i++)
            {
                length[i] = (list_human[i].bones[0].position - robot.bones[0].position).magnitude;
            }
            return vector;
        }

        //移動速度変化
        Vector3 Velocity()
        {
            Vector3 velocity = Vector3.zero;
            int human_Num = this.List_Human.Count;
            int human_Num_Last = this.List_Human_Last.Count;

            int count;
            if (human_Num < human_Num_Last) count = human_Num;
            else count = human_Num_Last;

            //人物移動距離の平均
            Vector3 vector = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                vector += (this.List_Human_Last[i].bones[0].position - this.List_Human[i].bones[0].position);
            }
            velocity = vector / Time.deltaTime;

            //vector = vector / vector.magnitude;
            return velocity;
        }

        //重心位置算出
        Vector3 CenterPosition(List<Human> list_human)
        {
            int human_Num = list_human.Count;
            Vector3 vector = new Vector3();
            for (int i = 0; i < list_human.Count; i++)
            {
                vector += list_human[i].bones[0].position;
            }

            if (this.IsHight)
            {
                //高さ成分あり 重心位置に出す
                vector /= human_Num;
                vector = new Vector3(vector.x, vector.y, vector.z);
            }
            else
            {
                //高さ成分無視
                vector = new Vector3(vector.x / human_Num, 0, vector.z / human_Num);
                vector = -vector;

            }

            //vector = vector / vector.magnitude;
            return vector;
        }
        Vector3 CulculateVelocity(Vector3 pos, Vector3 prePos)
        {
            Vector3 vel;
            vel = (pos - prePos) / Time.deltaTime;
            return vel;
        }
        Vector3 CulculateAcceleration(Vector3 vel, Vector3 preVel)
        {
            Vector3 acc;
            acc = (vel - preVel) / Time.deltaTime;
            return acc;
        }

        //public
        public void ChangeVel(bool Is)
        {
            this.IsChangeVelocity = Is;
        }
        public void ChangeIsHigh(bool Is)
        {
            this.IsHight = Is;
        }
        public void ChangeIsJumpVel(bool Is)
        {
            this.IsJumpVel = Is;
        }
        public void ChangeGravity(float gy)
        {
            Physics.gravity = new Vector3(0, -gy, 0);
        }
        public void ChangeJump(bool Is)
        {
            this.IsJump = Is;
        }
    }


}
