using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp.CPlusPlus;

namespace LabLife.Processer.ImageProcesser
{
    class ColorChanger : AImageProcesser
    {
        paramater param= new paramater();
        
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            this.Update(ref src, ref dst);
            
        }

        private void Update(ref Mat src, ref Mat dst)
        {
            Mat localdst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3, Scalar.Black);
            
            unsafe
            {
                
                int channel = localdst.Channels();
                byte* matPtr = src.DataPointer;
                byte* dstPtr = localdst.DataPointer;

                for (int i = 0; i < src.Width * src.Height ; i++)
                {
                    int b = *(matPtr + i * channel + 0);
                    int g = *(matPtr + i * channel + 1);
                    int r = *(matPtr + i * channel + 2);

                    int range = 30;
                    if (Math.Abs(b - 100) < range &&
                        Math.Abs(g - 100) < range &&
                        Math.Abs(r - 100) < range)
                    {
                        *(dstPtr + i * channel + 0) = 255;
                        *(dstPtr + i * channel + 1) = 0;
                        *(dstPtr + i * channel + 2) = 0;

                    }
                    
                }

                
            }
            dst = localdst.Clone();
        }

        public override string ToString()
        {
            return "ColorChanger";
        }
        public bool IsFirstFrame { get; private set; }
    }


}
