using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.ImageProcesser
{
    public abstract class AImageProcesser
    {
        public abstract void ImageProcess(ref Mat src, ref Mat dst);
    }
}
