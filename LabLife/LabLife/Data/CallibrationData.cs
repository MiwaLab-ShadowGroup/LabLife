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
        public List<float> srcx;
        public List<float> srcy;
        public List<float> dstx;
        public List<float> dsty;

        public CallibrationData()
        {
            srcx = new List<float>();
            srcy = new List<float>();
            dstx = new List<float>();
            dsty = new List<float>();
        }

        public override void setData(AData adata)
        {
            var srcdata = adata as CallibrationData;
            this.srcx = srcdata.srcx;
            this.srcy = srcdata.srcy;
            this.dstx = srcdata.dstx;
            this.dsty = srcdata.dsty;
        }

        public void SaveCallibration(string path, Point2f[] src, Point2f[] dst)
        {
            this.srcx = new List<float>();
            this.srcy = new List<float>();
            this.dstx = new List<float>();
            this.dsty = new List<float>();

            foreach (var p in src)
            {
                this.srcx.Add(p.X);
                this.srcy.Add(p.Y);
            }
            foreach (var q in src)
            {
                this.dstx.Add(q.X);
                this.dsty.Add(q.Y);
            }
            this.Save(path);
        }

        public void LoadCallibration(string path, Point2f[] src, Point2f[] dst)
        {
            this.Load(path);

            src[0].X = srcx[0];
            src[1].X = srcx[1];
            src[2].X = srcx[2];
            src[3].X = srcx[3];
            src[0].Y = srcy[0];
            src[1].Y = srcy[1];
            src[2].Y = srcy[2];
            src[3].Y = srcy[3];

            dst[0].X = dstx[0];
            dst[1].X = dstx[1];
            dst[2].X = dstx[2];
            dst[3].X = dstx[3];
            dst[0].Y = dsty[0];
            dst[1].Y = dsty[1];
            dst[2].Y = dsty[2];
            dst[3].Y = dsty[3];
        }
    }
}
