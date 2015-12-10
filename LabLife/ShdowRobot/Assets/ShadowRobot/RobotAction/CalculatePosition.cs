using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public delegate void _eventhandler(List<Human> list_human);
/// <summary>
/// 計算用の関数群
/// </summary>
public class CalculatePosition
{

    int boneNum;

    public CalculatePosition(int bonenum)
    {
        this.boneNum = boneNum;
    }
    public CalculatePosition()
    {
    }
    public void ChangeBoneNum(int bonenum)
    {
        this.boneNum = boneNum;
    }

    //重心位置算出
    public Vector3 CenterPosition(List<Human> list_human)
    {
        int human_Num = list_human.Count;
        Vector3 vector = new Vector3();
        for (int i = 0; i < list_human.Count; i++)
        {
            vector += list_human[i].bones[this.boneNum].position;
        }
        //高さ成分無視
        vector /= human_Num;
        return vector;
    }
    public Vector3 CenterPosition(List<Vector3> list_pos)
    {
        int human_Num = list_pos.Count;
        Vector3 vector = new Vector3();
        for (int i = 0; i < list_pos.Count; i++)
        {
            vector += list_pos[i];
        }
        vector /= human_Num;
        return vector;
    }
    //速度
    public Vector3 CulculateVelocity(Vector3 pos, Vector3 prePos)
    {
        Vector3 vel;
        vel = (pos - prePos) / Time.deltaTime;
        return vel;
    }
    //加速度
    public Vector3 CulculateAcceleration(Vector3 vel, Vector3 preVel)
    {
        Vector3 acc;
        acc = (vel - preVel) / Time.deltaTime;
        return acc;
    }

    //標準偏差
    public double StandardDeviation(List<Human> list_human)
    {
        Vector3 avr = this.CenterPosition(list_human);

        double bunsan = 0;
        for (int i = 0; i < list_human.Count; i++)
        {
            bunsan += (list_human[i].bones[this.boneNum].position - avr).magnitude * (list_human[i].bones[this.boneNum].position - avr).magnitude;
        }
        bunsan /= list_human.Count;
        //標準偏差を返す
        return Math.Sqrt(bunsan);
    }
    public double StandardDeviation(List<Vector3> list_pos)
    {
        Vector3 avr = this.CenterPosition(list_pos);

        double bunsan = 0;
        for (int i = 0; i < list_pos.Count; i++)
        {
            bunsan += (list_pos[i] - avr).magnitude * (list_pos[i] - avr).magnitude;
        }
        bunsan /= list_pos.Count;
        //標準偏差を返す
        return Math.Sqrt(bunsan);
    }
    //相関 パラメトリックxとzの関係だから使えないかも
    public double Pearson(List<Human> list_human)
    {
        Vector3 avr = this.CenterPosition(list_human);
        double x = 0;
        double z = 0;
        double xz = 0;
        for (int i = 0; i < list_human.Count; i++)
        {
            x += Math.Pow((list_human[i].bones[this.boneNum].position.x - avr.x), 2);
            z += Math.Pow((list_human[i].bones[this.boneNum].position.z - avr.z), 2);
            xz += (list_human[i].bones[this.boneNum].position.x - avr.x) * (list_human[i].bones[this.boneNum].position.z - avr.z);
        }

        //相関係数を返す
        return xz / (x * z);
    }
    public double Pearson(List<Vector3> list_pos)
    {
        Vector3 avr = this.CenterPosition(list_pos);
        double x = 0;
        double z = 0;
        double xz = 0;
        for (int i = 0; i < list_pos.Count; i++)
        {
            x += Math.Pow((list_pos[i].x - avr.x), 2);
            z += Math.Pow((list_pos[i].z - avr.z), 2);
            xz += (list_pos[i].x - avr.x) * (list_pos[i].z - avr.z);
        }

        //相関係数を返す
        return xz / (x * z);
    }

    public float MaxLength(List<Vector3> list, ref int humannum0, ref int humannum1)
    {
        float length = 0;       
        for(int i= 0; i< list.Count; i++)
        {
            for(int j = i; j < list.Count; j++)
            {
                if ((list[j] - list[i]).magnitude > length)
                {
                    length = (list[j] - list[i]).magnitude;
                    humannum0 = i;
                    humannum1 = j;
                }
            }
        }
        return length;
    }
    public float MinLength(List<Vector3> list, ref int humannum0, ref int humannum1)
    {
        float length = 0;
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i; j < list.Count; j++)
            {
                if ((list[j] - list[i]).magnitude < length || length == 0)
                {
                    length = (list[j] - list[i]).magnitude;
                    humannum0 = i;
                    humannum1 = j;
                }
                   
            }
        }
        return length;
    }
}

public class TimeDelay
{
    //宣言
    List<Human> List_Human;
    List<List<Human>> List_PastListHuman;
    public event _eventhandler _robotact;
    int interval;
    //float rate;

    public TimeDelay(int sec)
    {
        Application.targetFrameRate = 60;
        this.List_Human = new List<Human>();
        this.List_PastListHuman = new List<List<Human>>();

        //初期値5s遅れ
        this.interval = sec;
    }

    public void UpDate(List<Human> list_human)
    {
        //データ取得して保存
        this.List_Human = list_human;
        this.List_PastListHuman.Add(this.List_Human);

        //十分な保存データがあるときはロボットを移動
        if (this.List_PastListHuman.Count > (this.interval * Application.targetFrameRate))
        {
            //一番古いデータで処理
            this._robotact(this.List_PastListHuman[0]);
            //データ消去
            this.List_PastListHuman.RemoveAt(0);
        }

    }

    //遅れ時間変更
    public void ChangeInterval(int interval)
    {
        this.List_Human = this.ChangeInterval(interval, this.List_Human);
    }
    List<Human> ChangeInterval(int interval, List<Human> orgList)
    {
        List<Human> list_human = new List<Human>();
        if (this.interval < interval)
        {
            int count = (this.interval - interval) * Application.targetFrameRate;
            for (int i = count; i < list_human.Count; i++)
            {
                list_human.Add(orgList[i]);
            }
            return list_human;
        }
        else return orgList;
    }

}

public class RandomBoxMuller
{
    private System.Random random;

    public RandomBoxMuller()
    {
        random = new System.Random(Environment.TickCount);
    }

    public RandomBoxMuller(int seed)
    {
        random = new System.Random(seed);
    }

    public double next(double mu = 0.0, double sigma = 1.0, bool getCos = true)
    {
        if (getCos)
        {
            double rand = 0.0;
            while ((rand = random.NextDouble()) == 0.0) ;
            double rand2 = random.NextDouble();
            double normrand = Math.Sqrt(-2.0 * Math.Log(rand)) * Math.Cos(2.0 * Math.PI * rand2);
            normrand = normrand * sigma + mu;
            return normrand;
        }
        else
        {
            double rand;
            while ((rand = random.NextDouble()) == 0.0) ;
            double rand2 = random.NextDouble();
            double normrand = Math.Sqrt(-2.0 * Math.Log(rand)) * Math.Sin(2.0 * Math.PI * rand2);
            normrand = normrand * sigma + mu;
            return normrand;
        }
    }

    public double[] nextPair(double mu = 0.0, double sigma = 1.0)
    {
        double[] normrand = new double[2];
        double rand = 0.0;
        while ((rand = random.NextDouble()) == 0.0) ;
        double rand2 = random.NextDouble();
        normrand[0] = Math.Sqrt(-2.0 * Math.Log(rand)) * Math.Cos(2.0 * Math.PI * rand2);
        normrand[0] = normrand[0] * sigma + mu;
        normrand[1] = Math.Sqrt(-2.0 * Math.Log(rand)) * Math.Sin(2.0 * Math.PI * rand2);
        normrand[1] = normrand[1] * sigma + mu;
        return normrand;
    }
}

