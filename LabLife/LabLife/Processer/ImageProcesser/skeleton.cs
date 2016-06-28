using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Blob;
using OpenCvSharp;


namespace LabLife.Processer.ImageProcesser
{
    public class skeleton : AImageProcesser
    {

        public override void ImageProcess(ref Mat src, ref Mat dst)
        {
            this.Update(ref src, ref dst);
        }

        Mat grayimage = new Mat();
        Mat dstMat = new Mat();
        Mat resizedMat = new Mat();

        int Height;
        int Width;
        int x1, y1;
        List<Point> lst = new List<Point>();
        bool f1, f2, f3;
        Point p;

        private void Update(ref Mat src, ref Mat dst)
        {
            int imgW = 80;
            int imgH = 60;
            int srcW = src.Width;
            int srcH = src.Height;

            Cv2.Resize(src, this.resizedMat, new Size(imgW, imgH), 0, 0, Interpolation.Linear);
            int channel = src.Channels();
            this.dstMat = new Mat(imgH, imgW, MatType.CV_8UC3, new Scalar(0,0,0));

            //Cv2.CvtColor(src, this.grayimage, OpenCvSharp.ColorConversion.BgrToGray);

            int[,] b = new int[imgH, imgW];

            //細線化(田村の方法)
            //除去（D）と例外（K）のパターン設定
            #region
            int[,,] d1 = {{{-1, 0, -1}, {-1, 1, -1}, {-1, -1, -1}},
                        {{-1, -1, -1}, {-1, 1, 0}, {-1, -1, -1}}};
            int[,,] k1 = {{{-1, 0, -1}, {-1, 1, 1}, {-1, 1, 0}},
                        {{0, 1, -1}, {1, 1, 0}, {-1, -1, -1}}};
            int[,,] d2 = {{{-1, -1, -1}, {-1, 1, -1}, {-1, 0, -1}},
                     {{-1, -1, -1}, {0, 1, -1}, {-1, -1, -1}}};
            int[,,] k2 = {{{0, 1, -1}, {1, 1, -1}, {-1, 0, -1}},
                        {{-1, -1, -1}, {0, 1, 1}, {-1, 1, 0}}};
            int[,,] kc = {{{-1, -1, -1}, {0, 1, 0}, {-1, 1, -1}},
                        {{-1, 0, -1}, {1, 1, -1}, {-1, 0, -1}},
                        {{-1, 1, -1}, {0, 1, 0}, {-1, -1, -1}},
                        {{-1, 0, -1}, {-1, 1, 1}, {-1, 0, -1}},
                        {{-1, -1, -1}, {0, 1, -1}, {1, 0, -1}},
                        {{1, 0, -1}, {0, 1, -1}, {-1, -1, -1}},
                        {{-1, 0, 1}, {-1, 1, 0}, {-1, -1, -1}},
                        {{-1, -1, -1}, {-1, 1, 0}, {-1, 0, 1}},
                        {{0, 1, 0}, {1, 1, 1}, {0, -1, 0}},
                        {{0, 1, 0}, {-1, 1, 1}, {0, 1, 0}},
                        {{0, -1, 0}, {1, 1, 1}, {0, 1, 0}},
                        {{0, 1, 0}, {1, 1, -1}, {0, 1, 0}}};

            //int配列に黒のピクセルは1，白のピクセルは0を入れる
            //画像をint配列に
            //Console.WriteLine("1");
            //Console.WriteLine(channel);
            #endregion

            #region
            unsafe
            {
                byte* Ptr = this.resizedMat.DataPointer;

                for (int i = 0; i < imgW * imgH; i++)
                {

                    if (*(Ptr + i * channel) == 255)
                    {
                        b[(i / imgW), (i % imgW)] = 1;
                    }
                    else
                    {
                        b[(i / imgW), (i % imgW)] = 0;
                    }

                }
            }
            #endregion

            // 細線化
            #region
            List<Point> lst = new List<Point>();
            bool f1, f2, f3;
            Point p;

            do
            {
                //パターン1
                lst.Clear();
                for (int y = 0; y < imgH; y++)
                {
                    for (int x = 0; x < imgW; x++)
                    {
                        //削除パターン1の検証（一致なら、F1 = True）
                        f1 = PatternCheck(x, y, b, d1);
                        //除外パターン1の検証（一致なら、F2 = True）
                        if (f1)
                        {
                            f2 = PatternCheck(x, y, b, k1);
                            if (f2) f1 = false;
                        }
                        //除外パターン共通の検証（一致なら、F3 = True）
                        if (f1)
                        {
                            f3 = PatternCheck(x, y, b, kc);
                            if (f3) f1 = false;
                        }
                        //削除リストに登録（此処で削除しては駄目）
                        if (f1) lst.Add(new Point(x, y));
                    }
                }
                //Console.WriteLine("p");
                //パターン2
                if (lst.Count > 0)
                {
                    //削除リストに登録された画素を削除
                    for (int i = 0; i < lst.Count; i++)
                    {
                        p = lst[i];
                        b[p.Y, p.X] = 0;
                    }
                    lst.Clear();
                    for (int y = 0; y < imgH; y++)
                    {
                        for (int x = 0; x < imgW; x++)
                        {
                            //削除パターン2の検証（一致なら、F1 = True）
                            f1 = PatternCheck(x, y, b, d2);
                            //除外パターン2の検証（一致なら、F2 = True）
                            if (f1)
                            {
                                f2 = PatternCheck(x, y, b, k2);
                                if (f2) f1 = false;
                            }
                            //除外パターン共通の検証（一致なら、F3 = True）
                            if (f1)
                            {
                                f3 = PatternCheck(x, y, b, kc);
                                if (f3) f1 = false;
                            }
                            //削除リストに登録（此処で削除しては駄目）
                            if (f1) lst.Add(new Point(x, y));
                        }
                    }
                    //削除リストに登録された画素を削除
                    if (lst.Count > 0)
                    {
                        for (int i = 0; i < lst.Count; i++)
                        {
                            p = lst[i];
                            b[p.Y, p.X] = 0;
                        }
                    }
                }
            } while (lst.Count > 0);
            #endregion

            //描画処理
            unsafe
            {
                byte* dstPtr = this.dstMat.DataPointer;
                for (int i = 0; i < imgW * imgH; i++)
                {
                    if (b[i / imgW, i % imgW] == 1)
                    {
                        *(dstPtr + i * channel + 0) = 255;
                        *(dstPtr + i * channel + 1) = 255;
                        *(dstPtr + i * channel + 2) = 255;
                    }

                }
            }
            //Console.WriteLine("OK");

            //dst = this.dstMat.Clone();
            Cv2.Resize(this.dstMat, dst, new Size(srcW, srcH), 0, 0, Interpolation.Linear);

            
            //Cv2.Dilate(dst, dst, m_element, null, 2);
            //Cv2.Erode(dst, dst, m_element, null, 2);
        }

        //  細線化サブ（パターンの一致検証、一致ならTrue）
        private bool PatternCheck(int x, int y, int[,] b, int[,,] p)
        {
            bool flg = true;
            int x1, y1;
            for (int h = 0; h < p.GetLength(0); h++)
            {
                flg = true;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        x1 = x + j; y1 = y + i;
                        if ((x1 >= 0 && x1 < b.GetLength(1)) && (y1 >= 0 && y1 < b.GetLength(0)))
                        {
                            if (p[h, i + 1, j + 1] >= 0)
                            {
                                if (p[h, i + 1, j + 1] != b[y1, x1]) flg = false;
                            }
                        }
                        if (!flg) break;
                    }
                    if (!flg) break;
                }
                if (flg) break;
            }
            return flg;
        }

        public override string ToString()
        {
            return "Skeleton";
        }
        public bool IsFirstFrame { get; private set; }

    }
}
