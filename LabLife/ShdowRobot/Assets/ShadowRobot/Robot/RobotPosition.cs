using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Robot
{
    class RobotPosition 
    {
        CalculatePosition CP;

        //モード
        bool IsChangeVelocity;
        bool IsHight;
        bool IsJumpVel;
        bool IsJump;

        public RobotPosition()
        {
            this.CP = new CalculatePosition(0);

            this.IsChangeVelocity = false;
            this.IsHight = false;
            this.IsJumpVel = false;
            this.IsJump = false;
        }

        public Vector3 Action(List<Human> list_human) 
        {
            return this.MoveByCenterPosition(list_human);
            
        }
        public Vector3 Action(List<Vector3> list_pos)
        {
            return this.MoveByCenterPosition(list_pos);
        }

        //目標値
        //重心の線対象に移動
        Vector3 MoveByCenterPosition(List<Human> list_human)
        {
            Vector3 centerVec = this.CP.CenterPosition(list_human);//+ new Vector3(0, 0, 0.5f);
            //重心の原点点対称にだす
            if (this.IsHight) centerVec = new Vector3(-centerVec.x, centerVec.y, -centerVec.z);
            else centerVec = new Vector3(-centerVec.x, 0, -centerVec.z);

            return centerVec;

            //Vector3 vec = centerVec - robot.transform.position;
            //vec = vec / vec.magnitude;
            ////robot.GetComponent<Rigidbody>().AddForce(this.MoveByCenterPosition(list_human, robot));
            //robot.transform.position += vec;
        }
        Vector3 MoveByCenterPosition(List<Vector3> list_pos)
        {
            Vector3 centerVec = this.CP.CenterPosition(list_pos);//+ new Vector3(0, 0, 0.5f);
            //重心の原点点対称にだす
            if (this.IsHight) centerVec = new Vector3(-centerVec.x, centerVec.y, -centerVec.z);
            else centerVec = new Vector3(-centerVec.x, 0, -centerVec.z);

            return centerVec;
            //Vector3 vec = centerVec - robot.transform.position;
            //vec = vec / vec.magnitude;
            //robot.GetComponent<Rigidbody>().AddForce(this.MoveByCenterPosition(list_human, robot));
            //robot.transform.position += vec;
        }
        //ID0の人の後ろに出る
        //人とロボットの関係になるのでひとひねり
        Vector3 Trace(List<Human> list_human, Vector3 vec)
        {
            Vector3 vector;

            Vector3 pos = list_human[0].bones[0].position;
            Quaternion quat = list_human[0].bones[0].quaternion;

            //腰の位置に対して相対位置ずらした位置を目標にする。
            vector = pos + quat * vec;

            return vector;
        }

        //パラメータ
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
        public void ChangeJump(bool Is)
        {
            this.IsJump = Is;
        }
    }
}
