using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp.CPlusPlus;
namespace LabLife.Data
{
    public class CallibrationData : AData
    {
        public List<float> src;
        public List<float> dst;

        public CallibrationData()
        {
            src = new List<float>();
            dst = new List<float>();
        }

        public override void setData(AData adata)
        {
            var srcdata = adata as CallibrationData;
            this.src = srcdata.src;
            this.dst = srcdata.dst;
        }

        public void SaveCallibration(Point2f[] src, Point2f[] dst)
        {
            src[0] = new Point2f(src[0].X, src[0].Y);
            src[1] = new Point2f(src[1].X, src[1].Y);
            src[2] = new Point2f(src[2].X, src[2].Y);
            src[3] = new Point2f(src[3].X, src[3].Y);
        }

        public void LoadCallibration(Point2f[] src, Point2f[] dst)
        {

        }
    }
}
