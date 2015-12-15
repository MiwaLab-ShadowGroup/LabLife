using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp.CPlusPlus;

namespace LabLife.Processer.ImageProcesser
{
    class polygon : AImageProcesser
    {
        
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        int sharpness = 50;
        private Mat grayimage = new Mat();
        // Mat dstMat = new Mat()
        Random rand = new Random();

        private void Update(ref Mat src, ref Mat dst)
        {
            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            dst = new Mat(dst.Height, dst.Width, MatType.CV_8UC3,Scalar.Black);
                          
            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);
           

            List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();
            
            
            for (int i = 0; i < contour.Length; i++)
            {
                if(Cv2.ContourArea(contour[i]) > 100)
                {
                    CvPoints.Clear();

                    for (int j = 0; j < contour[i].Length; j += this.sharpness)
                    {

                        CvPoints.Add(contour[i][j]);
                    }
                    
                    Cv2.FillConvexPoly(dst, CvPoints, Scalar.Yellow,  OpenCvSharp.LineType.Link4, 0);
                }
                
            }
            //Console.WriteLine(contour.Length);
        }

        public override string ToString()
        {
            return "Polygon";
        }
        public bool IsFirstFrame { get; private set; }
    }

  
}
