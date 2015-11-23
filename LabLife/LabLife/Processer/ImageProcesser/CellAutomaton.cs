using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp.CPlusPlus;

namespace LabLife.Processer.ImageProcesser
{
    public class CellAutomaton : AImageProcesser
    {
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            
        }
        public override string ToString()
        {
            return "CellAutomaton";
        }
    }
}
