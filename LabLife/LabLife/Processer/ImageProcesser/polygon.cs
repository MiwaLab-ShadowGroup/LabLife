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

        int sharpness = 20;
        private Mat grayimage = new Mat();

        private void Update(ref Mat src, ref Mat dst)
        {
            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);
            Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.CComp, OpenCvSharp.ContourChain.ApproxSimple);

            List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<Point>();


            for (int i = 0; i < contour.Length; i++)
            {
                CvPoints.Clear();

                for (int j = 0; j < contour[i].Length; j += this.sharpness)
                {
                    
                    CvPoints.Add(contour[i][j]);
                }

                Cv2.FillConvexPoly(dst, CvPoints, Scalar.Yellow, OpenCvSharp.LineType.Link8, 0);
            }

        }
        public override string ToString()
        {
            return "Polygon";
        }
        public bool IsFirstFrame { get; private set; }
    }

}
