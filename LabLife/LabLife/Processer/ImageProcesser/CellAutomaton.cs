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

        private Mat m_element;
        private int __w;
        private int __h;

        public bool IsFirstFrame { get; private set; }

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            if (!IsInit)
            {
                this.Initialize(src.Width, src.Height);
                this.IsInit = true;
            }
            this.Update(ref src, ref dst);
        }

        #region CAmethods


        private void LifeGame_setup(int w, int h)
        {
            m_Field.Clear();
            __w = w;
            __h = h;

            for (int i = 0; i < h; i++)
            {
                List<bool> vTmp = new List<bool>();
                for (int j = 0; j < w; j++)
                {
                    vTmp.Add(false);
                }
                m_Field.Add(vTmp);
            }


            //field[10][10] = ALIVE;
            //field[10][11] = ALIVE;
            //field[10][12] = ALIVE;

            //field[11][10] = ALIVE;

            //field[12][11] = ALIVE;

            //bufimage_pre.allocate(w, h);
        }

        private void LifeGame_setCellFromImage(ref Mat image)
        {
            Mat bufimage = image.Clone();

            Cv2.Resize(bufimage, bufimage, new Size(__w, __h));
            //bufimage_pre.resize(_w,_h);

            //bufimage += bufimage_pre;

            unsafe
            {
                byte* pPixel = bufimage.DataPointer;
                Random r = new Random();
                for (int y = 0; y < __h; y++)
                {
                    for (int x = 0; x < __w; x++)
                    {
                        //黒くない点は生きてる
                        if (*pPixel != 0)
                        {
                            if (r.Next(0, 100) < 60)
                            {           //ofRandom(0,100) < 80だった
                                m_Field[y][x] = true;
                            }
                        }
                        /*else{
                            if( ofRandom(0,100) < 0 ){			//ofRandom(0,100) < 2だった
                                field[y][x] = ALIVE;
                            }
                            else if( ofRandom(0,100) > 0){	//ofRandom(0,100) < 2だった
                                field[y][x] = DEAD;
                            }
                        }*/
                        pPixel++;
                    }
                }
            }
            //bufimage_pre = bufimage;
            //bufimage_pre -= 20.0;
        }

        private bool LifeGame_getNextCell(int x, int y)
        {
            bool bRet = false;
            int aliveNum = 0;
            int curX = 0;
            int curY = 0;

            //自分のまわりの生きてるセルを数えます
            for (int px = -1; px <= 1; px++)
            {               //int px = -1; px <= 1; px++
                for (int py = -1; py <= 1; py++)
                {           //int py = -1; py <= 1; py++
                    if (px != 0 || py != 0)
                    {
                        curX = x + px;
                        curY = y + py;

                        if (curX < 0) curX = __w - 1;
                        if (curX > __w - 1) curX = 0;
                        if (curY < 0) curY = __h - 1;
                        if (curY > __h - 1) curY = 0;

                        if (m_Field[curY][curX] == true)
                        {
                            aliveNum++;
                        }
                    }
                }
            }

            if (m_Field[y][x] == true)
            {
                if (aliveNum <= 2 || aliveNum >= 4)
                {       //aliveNum <= 1 || aliveNum >= 4だった
                    bRet = false;
                }
                else
                {
                    bRet = true;
                }
            }
            else
            {
                if (aliveNum == 3)
                {       // aliveNum == 3だった
                    bRet = true;
                }
                else
                {
                    bRet = false;
                }
            }

            return bRet;
        }

        private void LifeGame_next()
        {
            List<List<bool>> tempField = new List<List<bool>>(m_Field);

            for (int x = 0; x < __w; x++)
            {
                for (int y = 0; y < __h; y++)
                {
                    tempField[y][x] = LifeGame_getNextCell(x, y);
                }
            }

            m_Field = tempField;
        }

        //private void LifeGame_draw(float x, float y, float w, float h)
        //{
        //    ofPushMatrix();
        //    ofPushStyle();

        //    ofTranslate(x, y);
        //    ofScale(w / (float)_w, h / (float)_h);

        //    for (int y = 0; y < _h; y++)
        //    {
        //        for (int x = 0; x < _w; x++)
        //        {
        //            if (field[y][x] == ALIVE)
        //            {
        //                ofSetColor(0xffffff);
        //            }
        //            else
        //            {
        //                ofSetColor(0x000000);
        //            }

        //            ofRect(x, y, 1, 1);
        //        }
        //    }

        //    ofPopMatrix();
        //    ofPopStyle();
        //}

        private void LifeGame_drawGrayscaleImage(ref Mat dst)
        {
            Mat img1;
            //img = cvCreateImage (cvGetSize (dst.getCvImage()), IPL_DEPTH_16S, 1);
            img1 = new Mat(new Size(__w, __h), MatType.CV_8UC1, new Scalar(0));


            unsafe
            {
                byte* dataptr = img1.DataPointer;
                for (int y = 0; y < __h; y++)
                {
                    for (int x = 0; x < __w; x++)
                    {
                        if (m_Field[y][x] == true)
                        {
                            dataptr[img1.Width * y + x] = 0xff;
                        }
                        else
                        {
                            dataptr[img1.Width * y + x] = 0x00;
                        }
                    }
                }
            }

            Cv2.Resize(img1, dst, dst.Size());
            //cvResize (img1, img2, CV_INTER_LINEAR);

        }


        #endregion

        private void Update(ref Mat src, ref Mat dst)
        {
            try
            {
                Cv2.CvtColor(src, bufimage, OpenCvSharp.ColorConversion.BgrToGray);
                Cv2.Laplacian(bufimage, bufimage, MatType.CV_8UC1);
                if (IsFirstFrame)
                {
                    this.LifeGame_setCellFromImage(ref bufimage);
                    this.IsFirstFrame = false;
                }
                else
                {

                    this.LifeGame_setCellFromImage(ref bufimage);
                }
                this.LifeGame_next();
                this.LifeGame_drawGrayscaleImage(ref bufimage);
                ///

                //unsafe
                //{
                //    byte* pPixel = bufimage.DataPointer;
                //    Random r = new Random();

                //    for (int y = 0; y < _h; y++)
                //    {
                //        for (int x = 0; x < _w; x++)
                //        {
                //            //黒くない点は生きてる
                //            if (*pPixel > 100)
                //            {
                //                m_Field[y][x] = true;
                //                if (r.Next(0, 100) < 60)
                //                {           //ofRandom(0,100) < 80だった
                //                    //m_Field[y][x] = true;
                //                }
                //            }
                //            else
                //            {
                //                m_Field[y][x] = false;
                //            }
                //            *pPixel = m_Field[y][x] ? (byte)255 : (byte)0;
                //            pPixel++;
                //        }
                //    }
                //}

                //    //////

                Cv2.Dilate(bufimage, bufimage, m_element);
                Cv2.Dilate(bufimage, bufimage, m_element);

                Cv2.Erode(bufimage, bufimage, m_element);
                Cv2.Erode(bufimage, bufimage, m_element);



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
                outerGrayBuffer2 -= 10;   //param.slider[1];

                outerGrayBuffer2 += tmp_bufImage_next3;

                innerGrayBuffer2 += tmp_bufImage_next3.Clone() - 230.0;


                for (int i = 0; i < 3; i++)
                {       //(int)param.slider[2]
                    Cv2.Erode(innerGrayBuffer2, innerGrayBuffer2, m_element);
                }

                for (int i = 0; i < 1; i++)
                {       //(int)param.slider[3]
                    Cv2.Erode(outerGrayBuffer2, outerGrayBuffer2, m_element);
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
            this.innerGrayBuffer2 = new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
            this.outerGrayBuffer2 = new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
            this.outerColorBuffer2 = new Mat(new Size(_w, _h), MatType.CV_8UC3, new Scalar(255, 255, 255));
            this.innerColorBuffer2 = new Mat(new Size(_w, _h), MatType.CV_8UC3, new Scalar(255, 255, 255));


            this.m_element = new Mat(3, 3, MatType.CV_8UC1, new Scalar(1));
            this.m_element.Set<byte>(0, 0, 0);
            this.m_element.Set<byte>(2, 0, 0);
            this.m_element.Set<byte>(0, 2, 0);
            this.m_element.Set<byte>(2, 2, 0);

            this.bufimage = new Mat(new Size(_w, _h), MatType.CV_8UC1, new Scalar(0));

           　LifeGame_setup(_w / 12, _h / 12);
            //for (int i = 0; i < h; i++)
            //{
            //    List<bool> vTmp = new List<bool>();
            //    for (int j = 0; j < w; j++)
            //    {
            //        vTmp.Add(false);
            //    }
            //    this.m_Field.Add(vTmp);
            //}
            this.IsFirstFrame = true;
        }

        public override string ToString()
        {
            return "CellAutoMaton";
        }
    }
}
