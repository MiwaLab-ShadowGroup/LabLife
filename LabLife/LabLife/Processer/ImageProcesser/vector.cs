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

        CvGraph grapth;
        CvSize size;
        //int interval;
        Mat thin;
        Mat raster_r;

        /// <summary>
        /// toUI
        /// </summary>
        int interval = 15;
        int length = 7;
        int radius = 1;
        Scalar color = new Scalar(255, 255, 0);
        Scalar colorBack;

        int count = 0;

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {

            this.Update(ref src, ref dst);

        }

        Mat grayimage = new Mat();


        private void Update(ref Mat src, ref Mat dst)
        {
            Mat dstMat = new Mat(src.Height, src.Width, MatType.CV_8UC3,new Scalar(0,0,0));



            int imgW = src.Width;
            int imgH = src.Height;


            Cv2.CvtColor(src, grayimage, OpenCvSharp.ColorConversion.BgrToGray);


            OpenCvSharp.CPlusPlus.Point[][] contour;
            HierarchyIndex[] hierarchy;
           

            Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

            Random rand = new Random();
            double x = 0;
            double y = 0;
            double dir;
            int size = 0;
            OpenCvSharp.CPlusPlus.Point center;
            for (int i = 0; i < contour.Length; i++)
            {
                if (Cv2.ContourArea(contour[i]) > 100)
                {
                    for (int j = 0; j < contour[i].Length - this.interval; j = j + this.interval)
                    {
                        OpenCvSharp.CPlusPlus.Point vec = contour[i][j + this.interval] - contour[i][j];
                        size = (int)Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y);
                        dir = (int)(Math.Atan2(vec.Y, vec.X) * 180);
                        //dir = rand.Next(dir - this.rangeRad, dir + this.rangeRad);
                        x = Math.Cos(dir);
                        y = Math.Sin(dir);


                        for (int k = 0; k < this.length; k++)
                        {
                            center = contour[i][j] + new OpenCvSharp.CPlusPlus.Point(x * k, y * k);
                            dstMat.Circle(center, 1, this.color, this.radius, LineType.Link8, 0);
                        }
                        for (int k = 0; k < this.length; k++)
                        {
                            center = contour[i][j] + new OpenCvSharp.CPlusPlus.Point(-x * k, -y * k);
                            dstMat.Circle(center, 1, this.color, this.radius, LineType.Link8, 0);
                        }

                    }
                }
            }
           
            dst = dstMat;

        }


        public override string ToString()
        {
            return "Vector";
        }
        public bool IsFirstFrame { get; private set; }

    }
}
