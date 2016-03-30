using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp.CPlusPlus;

namespace LabLife.Processer.ImageProcesser
{
    public class Timedelay : AImageProcesser
    {
        private Parameter DelayConter;

        int count = 0;

        Queue<Mat> queue;


        public Timedelay()
            :base()
        {

            DelayConter = new Parameter(ParameterType.Int, 100);
            //データの登録

            this.getParameter().Add(this.DelayConter);

            this.queue = new Queue<Mat>();


        }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        

        private void Update(ref Mat src, ref Mat dst)
        {
            Mat item = new Mat();
            src.CopyTo(item);


            this.queue.Enqueue(item);
            count++;

            if (count > (int)this.DelayConter.Value) //この値で遅れ時間を調整(UIで変えられる)
            {
                this.queue.Dequeue().CopyTo(dst);
                
            }

        }
        public override string ToString()
        {
            return "Timedelay";
        }
        public bool IsFirstFrame { get; private set; }

    }
}
