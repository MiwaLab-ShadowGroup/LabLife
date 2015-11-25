using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp;

namespace LabLife.Processer.ImageProcesser
{
    public class CellAutomaton : AImageProcesser
    {
        private List<List<bool>> m_Field = new List<List<bool>>();
        private bool IsInit = false;
        private int _w;
        private int _h;

        private Mat bufimage = new Mat();
        //永続確定
        private Mat outerColorBuffer2;
        private Mat innerColorBuffer2;
        private Mat innerGrayBuffer2;
        private Mat outerGrayBuffer2;
        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            if (!IsInit)
            {
                this.Initialize(src.Width, src.Height);
                this.IsInit = true;
            }
            this.Update(ref src, ref dst);
        }

        private void Update(ref Mat src, ref Mat dst)
        {
            try
            {
                Cv2.CvtColor(src, bufimage, OpenCvSharp.ColorConversion.BgrToGray);


                unsafe
                {
                    byte* pPixel = bufimage.DataPointer;
                    Random r = new Random();

                    for (int y = 0; y < _h; y++)
                    {
                        for (int x = 0; x < _w; x++)
                        {
                            //黒くない点は生きてる
                            if (*pPixel > 100)
                            {
                                m_Field[y][x] = true;
                                if (r.Next(0, 100) < 60)
                                {           //ofRandom(0,100) < 80だった
                                    //m_Field[y][x] = true;
                                }
                            }
                            else
                            {
                                m_Field[y][x] = false;
                            }
                            *pPixel = m_Field[y][x] ? (byte)255 : (byte)0;
                            pPixel++;
                        }
                    }
                }


                /////



                //    //////

                Cv2.Dilate(bufimage, bufimage, new Mat());
                Cv2.Dilate(bufimage, bufimage, new Mat());
                Cv2.Dilate(bufimage, bufimage, new Mat());
                Cv2.Dilate(bufimage, bufimage, new Mat());
                Cv2.Dilate(bufimage, bufimage, new Mat());
                Cv2.Dilate(bufimage, bufimage, new Mat());

                Cv2.Erode(bufimage, bufimage, new Mat());
                Cv2.Erode(bufimage, bufimage, new Mat());
                Cv2.Erode(bufimage, bufimage, new Mat());
                Cv2.Erode(bufimage, bufimage, new Mat());
                Cv2.Erode(bufimage, bufimage, new Mat());
                Cv2.Erode(bufimage, bufimage, new Mat());
                


                //*********************************************************************************************
                //ここからメイン
                //*********************************************************************************************

                ////***********************************************************

                ////cvFindContoursを用いた輪郭抽出*****************************
                Mat tmp_bufImage_next;
                Mat tmp_bufImage_next3;

                //TODO:移動可
                tmp_bufImage_next = new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
                tmp_bufImage_next3 = new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));

                bufimage.CopyTo(tmp_bufImage_next);

                Point[][] contours;
                HierarchyIndex[] hierarchy;

                /// Find contours
                Cv2.FindContours(tmp_bufImage_next, out contours, out hierarchy, OpenCvSharp.ContourRetrieval.Tree, OpenCvSharp.ContourChain.ApproxNone);

                /// Draw contours
                for (int i = 0; i < contours.Length; i++)
                {
                    Scalar color = new Scalar(255);
                    //Cv2.DrawContours(tmp_bufImage_next3, contours, i, color, 2, OpenCvSharp.LineType.Link8, hierarchy, 0);
                    Cv2.FillPoly(tmp_bufImage_next3, contours, color);
                }


                //cvClearSeq(contours);	//これはいらないみたい
                ////***********************************************************

                ////残像処理***************************************************

                
                innerGrayBuffer2 -= 0.2;        //param.slider[0];
                outerGrayBuffer2 -= 10.0;   //param.slider[1];

                outerGrayBuffer2 += tmp_bufImage_next3;

                innerGrayBuffer2 += tmp_bufImage_next3.Clone() - 230.0;


                for (int i = 0; i < 3; i++)
                {       //(int)param.slider[2]
                    Cv2.Erode(innerGrayBuffer2, innerGrayBuffer2, new Mat());
                }

                for (int i = 0; i < 1; i++)
                {       //(int)param.slider[3]
                    Cv2.Erode(outerGrayBuffer2, outerGrayBuffer2, new Mat());
                }



                Mat tmpColorBuffer2 = new Mat(new Size(bufimage.Width, bufimage.Height), MatType.CV_8UC3, new Scalar(255, 255, 255));
                outerColorBuffer2.SetTo(new Scalar(255, 255, 255));
                Cv2.CvtColor(outerGrayBuffer2, tmpColorBuffer2, OpenCvSharp.ColorConversion.GrayToBgr);
                Cv2.Multiply(outerColorBuffer2, tmpColorBuffer2, outerColorBuffer2, 1.0 / 255.0);
                innerColorBuffer2.SetTo(new Scalar(255, 255, 255));
                Cv2.CvtColor(innerGrayBuffer2, tmpColorBuffer2, OpenCvSharp.ColorConversion.GrayToBgr);
                Cv2.Multiply(innerColorBuffer2, tmpColorBuffer2, innerColorBuffer2, 1.0 / 255.0);

                outerColorBuffer2 -= innerColorBuffer2;
                outerColorBuffer2.GaussianBlur(new Size(3, 3), 3).CopyTo(dst);
                ////***********************************************************
                //bufimage_pre = bufimage;
                //bufimage_pre -= 20.0;




                //Cv2.CvtColor(bufimage, dst, OpenCvSharp.ColorConversion.GrayToBgr);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.TargetSite);
            }


        }

        private void Initialize(int w, int h)
        {
            m_Field.Clear();
            _w = w;
            _h = h;
            this.innerGrayBuffer2= new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
            this.outerGrayBuffer2 = new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
            this.outerColorBuffer2 = new Mat(new Size(_w, _h), MatType.CV_8UC3, new Scalar(255, 255, 255));
            this.innerColorBuffer2 = new Mat(new Size(_w, _h), MatType.CV_8UC3, new Scalar(255, 255, 255));

            this.bufimage = new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
            for (int i = 0; i < h; i++)
            {
                List<bool> vTmp = new List<bool>();
                for (int j = 0; j < w; j++)
                {
                    vTmp.Add(false);
                }
                this.m_Field.Add(vTmp);
            }

        }

        public override string ToString()
        {
            return "CellAutomaton";
        }
    }
}
