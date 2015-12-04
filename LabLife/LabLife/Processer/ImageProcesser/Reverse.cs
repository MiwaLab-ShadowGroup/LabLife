using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp.CPlusPlus;

namespace LabLife.Processer.ImageProcesser
{
    public class Reverse : AImageProcesser
    {
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            dst = ~src;
        }
        public override string ToString()
        {
            return "画像反転";
        }
    }
}
