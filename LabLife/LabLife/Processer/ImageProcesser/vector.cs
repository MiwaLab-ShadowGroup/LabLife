using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp.CPlusPlus;
using System.Windows.Forms;
using System.Drawing;
using OpenCvSharp;

namespace LabLife.Processer.ImageProcesser
{
    public class vector : AImageProcesser
    {


        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        private Mat grayimage = new Mat();


        private void Update(ref Mat src, ref Mat dst)
        {

            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            IplImage vectoripl = new IplImage(dst.Width, dst.Height, BitDepth.U8, 3);
            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3);

            OpenCvSharp.CPlusPlus.Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;
            CvScalar color = new CvScalar(255, 0, 0);

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

            List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<OpenCvSharp.CPlusPlus.Point>();

            //Console.Write(contour.Length);

            for (int i = 0; i < contour.Length; i++)
            {
                //Console.Write(contour[i].Length);

                if (Cv2.ContourArea(contour[i]) > 100)
                {
                    CvPoints.Clear();

                    for (int j = 0; j < contour[i].Length; j += 30)
                    {

                        CvPoints.Add(contour[i][j]);
                    }

                }

            }
            //Console.WriteLine(CvPoints.Count);
            //Cv.Line(vectoripl, CvPoints[0], CvPoints[1], new CvScalar(255, 255, 0), 2);
            
            for (int k = 0; k < CvPoints.Count; k+=2)
            {

                Cv.Line(vectoripl, CvPoints[k], CvPoints[k + 1], color, 2);

            }

            Mat dst1 = new Mat(vectoripl);
            dst = dst1;
        }


        public override string ToString()
        {
            return "Vector";
        }
        public bool IsFirstFrame { get; private set; }

    }
}
