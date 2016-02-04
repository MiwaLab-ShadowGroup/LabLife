using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
using System.Net.Sockets;
using System.Windows.Threading;
using System.IO;
using OpenCvSharp;
using OpenCvSharp.Blob;

//using System.Windows.Forms;

namespace Kinect2DShadow
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        //宣言
        #region
        KinectSensor kinect;
        //カラーイメージ
        ColorImageFormat colorImageFormat;
        ColorFrameReader colorFrameReader;
        FrameDescription colorFrameDescription;
        WriteableBitmap colorImg;
        Int32Rect bitmapRect;
        int bitmapStride;
        byte[] colors;
        int imageWidth;
        int imageHeigt;
        //BodyIndex
        BodyIndexFrameReader bodyIndexFrameReader;
        FrameDescription bodyIndexFrameDes;
        byte[] bodyIndexBuffer;
        int bodyIndexWidth;
        int bodyIndexHeight;
        Mat bodyIndexMat;

        Object SyncObject = new Object();

        //深度情報
        DepthFrameReader depthFrameReader;
        FrameDescription depthFrameDescription;
        ushort[] depthBuffer;
        CameraSpacePoint[] cameraSpacePoints;
        int depthImageWidth;
        int depthImageHeight;
        struct _space
        {
            public float Back;
            public float Front;
            public float Floor;
            public float Roof;
            public float Left;
            public float Right;
        }
        _space space;
        Mat depthMat;

        //CIPC
        CIPCClientWindow.MainWindow cipcMain;
        string preIP;
        int preServerPort;
        int preCLientPort;


        //UDP
        List<UdpClient> List_UDP;
        DispatcherTimer timer;
        List<string> List_IP;
        List<int> List_Port;
        
        #endregion

        //archive
        string FilePath;
        BinaryReader reader;
        int datalength;
        ushort[] readData;
        bool Isreader = true;
        bool ReadStop = false;
        Thread DepthThread;
        Thread ReadThread;
        FPSAdjuster.FPSAdjuster FpsAd;
        CameraSpacePoint[] cameraSpacePointsArchive;
        Mat Archivemat;
        bool IsArchive = false;
        Mat Compositionmat;
        bool endthread = false;

        //メディア
        int sharpness = 50;
        private Mat grayimage = new Mat();
        private Mat Archivegrayimage = new Mat();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_Contours = new List<List<OpenCvSharp.CPlusPlus.Point>>();
        List<List<OpenCvSharp.CPlusPlus.Point>> List_ArchiveContours = new List<List<OpenCvSharp.CPlusPlus.Point>>();

        Mat Polygon;
        Mat ArchivePolygon;
        Mat Zanzou;
        Mat ArchiveZanzou;

        bool IsPolygon = false;
        bool IsZanzou = false;

        private List<List<bool>> m_Field = new List<List<bool>>();
        private List<List<bool>> m_Field2 = new List<List<bool>>();
        
        private int _w;
        private int _h;

        private Mat bufimage = new Mat();
        private Mat bufimage2 = new Mat();

        //永続確定
        private Mat outerColorBuffer2;
        private Mat innerColorBuffer2;
        private Mat innerGrayBuffer2;
        private Mat outerGrayBuffer2;

        private Mat outerColorBuffer1;
        private Mat innerColorBuffer1;
        private Mat innerGrayBuffer1;
        private Mat outerGrayBuffer1;

        private Mat m_element;
        private Mat m_element2;

        private int __w;
        private int __h;
        int w = 512;
        int h = 424;

        //Color
        int red;
        int blue;
        int green;

        int outred;
        int outblue;
        int outgreen;


        Mat sendmat;
        //ラベリング
        bool bool_label = false;
        //Mat graylabel;

        CvBlobs blobs;
        IplImage labelimage;
        IplImage labelgrayimage;

        public MainWindow()
        {
            InitializeComponent();
            

            this.kinect = KinectSensor.GetDefault();
            //colorImage
            #region
            this.colorImageFormat = ColorImageFormat.Bgra;
            this.colorFrameDescription = this.kinect.ColorFrameSource.CreateFrameDescription(this.colorImageFormat);
            this.colorFrameReader = this.kinect.ColorFrameSource.OpenReader();
            this.colorFrameReader.FrameArrived += ColorFrame_Arrived;
            this.colors = new byte[this.colorFrameDescription.Width
                                           * this.colorFrameDescription.Height
                                           * this.colorFrameDescription.BytesPerPixel];
            this.imageWidth = this.colorFrameDescription.Width;
            this.imageHeigt = this.colorFrameDescription.Height;
            this.colorImg = new WriteableBitmap(this.colorFrameDescription.Width, this.colorFrameDescription.Height, 96, 96, PixelFormats.Bgr32, null);
            this.bitmapRect = new Int32Rect(0, 0, this.colorFrameDescription.Width, this.colorFrameDescription.Height);
            this.bitmapStride = this.colorFrameDescription.Width * (int)this.colorFrameDescription.BytesPerPixel;
            this.colorImage.Source = this.colorImg;
            #endregion
    
            //BodyIndex
            #region
            this.bodyIndexFrameDes = this.kinect.BodyIndexFrameSource.FrameDescription;
            this.bodyIndexFrameReader = this.kinect.BodyIndexFrameSource.OpenReader();
            this.bodyIndexFrameReader.FrameArrived += this.BodyIndexFrame_Arrived;
            this.bodyIndexBuffer = new byte[this.bodyIndexFrameDes.Width *
                                                this.bodyIndexFrameDes.Height * this.bodyIndexFrameDes.BytesPerPixel];
            this.bodyIndexWidth = this.bodyIndexFrameDes.Width;
            this.bodyIndexHeight = this.bodyIndexFrameDes.Height;
            this.bodyIndexMat = new Mat(this.bodyIndexHeight, this.bodyIndexWidth, MatType.CV_8UC3);

            #endregion

            //深度情報
            #region
            this.depthFrameReader = this.kinect.DepthFrameSource.OpenReader();
            this.depthFrameReader.FrameArrived += DepthFrame_Arrived;
            this.depthFrameDescription = this.kinect.DepthFrameSource.FrameDescription;
            this.depthBuffer = new ushort[this.depthFrameDescription.LengthInPixels];
            this.depthImageWidth = this.depthFrameDescription.Width;
            this.depthImageHeight = this.depthFrameDescription.Height;
            this.cameraSpacePoints = new CameraSpacePoint[this.depthFrameReader.DepthFrameSource.FrameDescription.LengthInPixels];
            this.space = new _space();
            this.space.Back  = float.Parse(this.Back.Text);
            this.space.Front = float.Parse(this.Front.Text);
            this.space.Floor = float.Parse(this.Floor.Text);
            this.space.Roof  = float.Parse(this.Roof.Text);
            this.space.Left  = float.Parse(this.Left.Text);
            this.space.Right = float.Parse(this.Right.Text);
            this.depthMat = new Mat(this.depthImageHeight, this.depthImageWidth, MatType.CV_8UC3);
            #endregion

            this.preIP = "127.0.0.1";
            this.preCLientPort = 54000;
            this.preServerPort = 50000;
            this.Closed += MainWindow_Closed;

            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromMilliseconds(40);
            this.timer.Tick += new EventHandler(this.SendImage);
            this.timer.Start();

            this.readData = new ushort[512 * 424];
            this.cameraSpacePointsArchive = new CameraSpacePoint[this.depthFrameReader.DepthFrameSource.FrameDescription.LengthInPixels];
            this.Archivemat = new Mat(this.depthImageHeight, this.depthImageWidth, MatType.CV_8UC3);
            this.Compositionmat = Archivemat.Clone();

            //List
            List_UDP = new List<UdpClient>();
            List_IP = new List<string>();
            List_Port = new List<int>();

            this.Polygon = Archivemat.Clone();
            this.ArchivePolygon = Archivemat.Clone();
            this.Zanzou = Archivemat.Clone();
            this.ArchiveZanzou = Archivemat.Clone();

            //残像用
            m_Field.Clear();
            _w = w;
            _h = h;
            this.innerGrayBuffer2 = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
            this.outerGrayBuffer2 = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
            this.outerColorBuffer2 = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC3, new Scalar(255, 255, 255));
            this.innerColorBuffer2 = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC3, new Scalar(255, 255, 255));
            this.innerGrayBuffer1 = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
            this.outerGrayBuffer1 = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
            this.outerColorBuffer1 = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC3, new Scalar(255, 255, 255));
            this.innerColorBuffer1 = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC3, new Scalar(255, 255, 255));

            this.m_element = new Mat(3, 3, MatType.CV_8UC1, new Scalar(1));
            this.m_element.Set<byte>(0, 0, 0);
            this.m_element.Set<byte>(2, 0, 0);
            this.m_element.Set<byte>(0, 2, 0);
            this.m_element.Set<byte>(2, 2, 0);

            this.m_element2 = new Mat(3, 3, MatType.CV_8UC1, new Scalar(1));
            this.m_element2.Set<byte>(0, 0, 0);
            this.m_element2.Set<byte>(2, 0, 0);
            this.m_element2.Set<byte>(0, 2, 0);
            this.m_element2.Set<byte>(2, 2, 0);

            this.bufimage = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
            this.bufimage2 = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC1, new Scalar(0));

            this.sendmat = new Mat(212, 256, MatType.CV_8UC3);

            this.blobs = new CvBlobs();
            this.labelgrayimage = new IplImage(512, 424, BitDepth.U8, 1);
            Cv.NamedWindow("label", WindowMode.AutoSize);



            for (int i = 0; i < h; i++)
            {
                List<bool> vTmp = new List<bool>();
                for (int j = 0; j < w; j++)
                {
                    vTmp.Add(false);
                }
                this.m_Field.Add(vTmp);
            }

            for (int i = 0; i < h; i++)
            {
                List<bool> vTmp = new List<bool>();
                for (int j = 0; j < w; j++)
                {
                    vTmp.Add(false);
                }
                this.m_Field2.Add(vTmp);
            }



        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                this.cipcMain.Close();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }  
     
        //Button
        void ConnectCIPC(object sender, RoutedEventArgs e)
        {
            //Connect
            if (this.cipcMain == null)
            {
                CIPCClientWindow.CIPCSettingWindow settingWindow = new CIPCClientWindow.CIPCSettingWindow(this.preIP, this.preServerPort, this.preCLientPort); 
                if (settingWindow.ShowDialog() == true)
                {
                    this.cipcMain = new CIPCClientWindow.MainWindow("CIPC", settingWindow);
                    this.cipcMain.Show();
                    this.CIPCButton.Content = "Close";
                    Console.WriteLine("eventhander");
                }
            }
            //Close
            else
            {
                try
                {
                    this.preIP = this.cipcMain.TextBlock_ServerIPAdress.Text;
                    this.preCLientPort = int.Parse(this.cipcMain.TextBlock_ClientPort.Text);
                    this.preServerPort = int.Parse(this.cipcMain.TextBlock_ServerPort.Text);
                    this.cipcMain.Close();
                    this.cipcMain = null;
                    this.CIPCButton.Content = "Connect";
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }
            
            
        }
        void ConnectUDP(object sender, RoutedEventArgs e)
        {
            List_UDP.Add(new UdpClient());
            List_IP.Add(this.UDPip.Text);
            List_Port.Add(int.Parse(this.UDPport.Text));

            this.message.Content = "ConnectUDP, " + this.UDPip.Text+ ", " + this.UDPport.Text;
        }
        
        void KinectButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.KinectButton.Content.ToString() == "Start")
            {
                //kinect Start
                try
                {
                    this.kinect.Open();
                    this.KinectButton.Content = "Stop";
                    //this.time = DateTimeOffset.Now.Millisecond;
                    if (this.kinect == null) this.kinect = KinectSensor.GetDefault();
                }
                catch
                {
                    this.ErrorPoint( System.Reflection.MethodBase.GetCurrentMethod().Name.ToString());
                }
            }
            else if (this.KinectButton.Content.ToString() == "Stop")
            {
                //kinect Stop
                try
                {
                    if (this.kinect != null) this.kinect.Close();
                    this.KinectButton.Content = "Start";
                }
                catch 
                {
                    this.ErrorPoint(System.Reflection.MethodBase.GetCurrentMethod().Name.ToString());
                }
                
            }
        } 
      
        //bodyIndexframe取得時のイベント
        void BodyIndexFrame_Arrived(object sender, BodyIndexFrameArrivedEventArgs e)
        {
            BodyIndexFrame bodyIndexFrame = e.FrameReference.AcquireFrame();
            if (bodyIndexFrame == null) return;
            bodyIndexFrame.CopyFrameDataToArray(bodyIndexBuffer);  //人がいないところ0xff いるところ0-6？
            this.BodyIndextoMat();
            //破棄
            bodyIndexFrame.Dispose();
        }
        void BodyIndextoMat()
        {

            int depth = this.bodyIndexMat.Depth();
            int channel = this.bodyIndexMat.Channels();
            unsafe
            {
                byte* matPtr = this.bodyIndexMat.DataPointer;
                for (int i = 0; i < this.bodyIndexWidth * this.bodyIndexHeight; i++)
                {
                    if (this.bodyIndexBuffer[i] == 255)
                    {
                        for (int j = 0; j < channel; j++)
                        {
                            *(matPtr + i * channel + j) = 0;

                        }
                    }
                    else
                    {
                        for (int j = 0; j < channel; j++)
                        {
                            *(matPtr + i * channel + j) = 255;

                        }
                    }

                }
            }

            this.bodyIndexImage.Source = WriteableBitmapConverter.ToWriteableBitmap(this.bodyIndexMat);

        }

        //深度情報取得
        void DepthFrame_Arrived(object sender, DepthFrameArrivedEventArgs e)
        {
            try
            {
                DepthFrame depthFrame = e.FrameReference.AcquireFrame();
                //フレームがなければ終了、あれば格納
                if (depthFrame == null) return;
                int[] depthBitdata = new int[depthBuffer.Length];
                depthFrame.CopyFrameDataToArray(this.depthBuffer);
                this.kinect.CoordinateMapper.MapDepthFrameToCameraSpace(this.depthBuffer, this.cameraSpacePoints);
                
                this.DepthToMat();

                
                //破棄
                depthFrame.Dispose();
            }
            catch
            {
                this.ErrorPoint(System.Reflection.MethodBase.GetCurrentMethod().ToString());
            }
        }
        void DepthToMat()
        {
            int channel = this.depthMat.Channels();
            unsafe
            {
                byte* matPtr = this.depthMat.DataPointer;

                for (int i = 0; i < this.cameraSpacePoints.Length; i++)
                {
                    //条件で絞る
                    if (this.cameraSpacePoints[i].Z < this.space.Back&&
                        this.cameraSpacePoints[i].Z > this.space.Front &&
                        this.cameraSpacePoints[i].Y > this.space.Floor&&
                        this.cameraSpacePoints[i].Y < this.space.Roof &&
                        this.cameraSpacePoints[i].X > this.space.Left&&
                        this.cameraSpacePoints[i].X < this.space.Right)
                    {
                        //条件クリア（人）
                        //for(int j= 0; j < channel ; j ++)
                        //{
                            *(matPtr + i * channel + 0) = (byte)RedSlider.Value;
                            *(matPtr + i * channel + 1) = (byte)GreenSlider.Value;
                            *(matPtr + i * channel + 2) = (byte)BlueSlider.Value;

                        //}
                    }
                    else
                    {
                        //人以外
                        for (int j = 0; j < channel; j++)
                        {
                            *(matPtr + i * channel + j) = 0;
                        }
                    }
                }
            }
            
            this.DepthImage.Source = WriteableBitmapConverter.ToWriteableBitmap(this.depthMat);
            if ((bool)checkBox_Polygon.IsChecked)
            {
                
                this.List_Contours.Clear();
                this.Polygon = new Mat(424, 512, MatType.CV_8UC3);
                Cv2.CvtColor(depthMat, grayimage, OpenCvSharp.ColorConversion.BgrToGray);

                OpenCvSharp.CPlusPlus.Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
                OpenCvSharp.CPlusPlus.HiearchyIndex[] hierarchy;

                Cv2.FindContours(grayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

                List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<OpenCvSharp.CPlusPlus.Point>();

                //Console.WriteLine(contour.Length);

                for (int i = 0; i < contour.Length; i++)
                {

                    CvPoints.Clear();

                    if (Cv2.ContourArea(contour[i]) > 1000)
                    {

                        for (int j = 0; j < contour[i].Length; j += this.sharpness)
                        {

                            CvPoints.Add(contour[i][j]);
                        }

                        this.List_Contours.Add(CvPoints);
                        //Cv2.FillConvexPoly(Polygon, CvPoints, Scalar.Yellow,  OpenCvSharp.LineType.Link4, 0);
                        Cv2.DrawContours(Polygon, this.List_Contours, 0, Scalar.FromRgb((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value), -1, OpenCvSharp.LineType.Link8);

                    }

                }
                   
                

            }

            if ((bool)checkBox_Zanzou.IsChecked)
            {
                try
                {
                    this.Zanzou = new Mat(424, 512, MatType.CV_8UC3);

                    Cv2.CvtColor(depthMat, bufimage2, OpenCvSharp.ColorConversion.BgrToGray);

                    unsafe
                    {
                        byte* pPixel = bufimage2.DataPointer;
                        Random r = new Random();

                        for (int y = 0; y < _h; y++)
                        {
                            for (int x = 0; x < _w; x++)
                            {
                                //黒くない点は生きてる
                                if (*pPixel > 100)
                                {
                                    m_Field2[y][x] = true;
                                    if (r.Next(0, 100) < 60)
                                    {           //ofRandom(0,100) < 80だった
                                                //m_Field[y][x] = true;
                                    }
                                }
                                else
                                {
                                    m_Field2[y][x] = false;
                                }
                                *pPixel = m_Field2[y][x] ? (byte)255 : (byte)0;
                                pPixel++;
                            }
                        }
                    }

                    //    //////

                    Cv2.Dilate(bufimage2, bufimage2, m_element2);
                    Cv2.Dilate(bufimage2, bufimage2, m_element2);

                    Cv2.Erode(bufimage2, bufimage2, m_element2);
                    Cv2.Erode(bufimage2, bufimage2, m_element2);


                    ////cvFindContoursを用いた輪郭抽出*****************************
                    Mat tmp_bufImage_next1;
                    Mat tmp_bufImage_next4;

                    //TODO:移動可
                    tmp_bufImage_next1 = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
                    tmp_bufImage_next4 = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC1, new Scalar(0));

                    bufimage2.CopyTo(tmp_bufImage_next1);

                    OpenCvSharp.CPlusPlus.Point[][] contours2;
                    OpenCvSharp.CPlusPlus.HiearchyIndex[] hierarchy2;

                    /// Find contours
                    Cv2.FindContours(tmp_bufImage_next1, out contours2, out hierarchy2, OpenCvSharp.ContourRetrieval.Tree, OpenCvSharp.ContourChain.ApproxNone);

                    /// Draw contours
                    for (int i = 0; i < contours2.Length; i++)
                    {
                        Scalar color = new Scalar(255);
                        //Cv2.DrawContours(tmp_bufImage_next3, contours, i, color, 2, OpenCvSharp.LineType.Link8, hierarchy, 0);
                        Cv2.FillPoly(tmp_bufImage_next4, contours2, color);
                    }

                    ////残像処理***************************************************

                    innerGrayBuffer1 -= 0.2;        //param.slider[0];
                    outerGrayBuffer1 -= 10;   //param.slider[1];

                    outerGrayBuffer1 += tmp_bufImage_next4;

                    innerGrayBuffer1 += tmp_bufImage_next4.Clone() - 230.0;

                    for (int i = 0; i < 3; i++)
                    {       //(int)param.slider[2]
                        Cv2.Erode(innerGrayBuffer1, innerGrayBuffer1, m_element2);
                    }

                    for (int i = 0; i < 1; i++)
                    {       //(int)param.slider[3]
                        Cv2.Erode(outerGrayBuffer1, outerGrayBuffer1, m_element2);
                    }


                    Mat tmpColorBuffer1 = new Mat(new OpenCvSharp.CPlusPlus.Size(bufimage2.Width, bufimage2.Height), MatType.CV_8UC3, new Scalar(255, 255, 255));
                    outerColorBuffer1.SetTo(new Scalar((int)RedSlider_Copy.Value, (int)GreenSlider_Copy.Value, (int)BlueSlider_Copy.Value));
                    Cv2.CvtColor(outerGrayBuffer1, tmpColorBuffer1, OpenCvSharp.ColorConversion.GrayToBgr);
                    Cv2.Multiply(outerColorBuffer1, tmpColorBuffer1, outerColorBuffer1, 1.0 / 255.0);
                    innerColorBuffer1.SetTo(new Scalar((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value));
                    Cv2.CvtColor(innerGrayBuffer1, tmpColorBuffer1, OpenCvSharp.ColorConversion.GrayToBgr);
                    Cv2.Multiply(innerColorBuffer1, tmpColorBuffer1, innerColorBuffer1, 1.0 / 255.0);

                    outerColorBuffer1 -= innerColorBuffer1;
                    outerColorBuffer1.GaussianBlur(new OpenCvSharp.CPlusPlus.Size(3, 3), 3).CopyTo(Zanzou);


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.TargetSite);
                }

            }

        }

        //カラーイメージ取得
        void ColorFrame_Arrived(object sender, ColorFrameArrivedEventArgs e)
        {
            try
            {
                ColorFrame colorFrame = e.FrameReference.AcquireFrame();
                //フレームがなければ終了、あれば格納
                if (colorFrame == null) return;
                colorFrame.CopyConvertedFrameDataToArray(this.colors, this.colorImageFormat);
                //表示
                this.colorImg.WritePixels(this.bitmapRect, this.colors, this.bitmapStride, 0);
               



                //破棄
                colorFrame.Dispose();
            }
            catch
            {
                this.ErrorPoint(System.Reflection.MethodBase.GetCurrentMethod().ToString());
            }

            
        }

        void SendImage(object obj, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (List_UDP.Count != 0)
                {
                    for (int i = 0; i < List_UDP.Count; i++)
                    {
                        if (this.List_UDP[i] != null)
                        {
                            byte[] data;

                            if ((bool)this.radioButton_BI.IsChecked)
                            {
                                data = this.bodyIndexMat.ToBytes(".jpg");
                                this.List_UDP[i].Send(data, data.Length, List_IP[i], List_Port[i]);
                            }
                            else if ((bool)this.radioButton_depth.IsChecked)
                            {
                                if ((bool)checkBox_Polygon.IsChecked == false && (bool)checkBox_Zanzou.IsChecked == false)
                                {

                                    if (!IsArchive)
                                    {
                                        Cv2.Resize(depthMat, sendmat, new OpenCvSharp.CPlusPlus.Size(sendmat.Width,sendmat.Height));
                                        data = this.sendmat.ToBytes(".jpg");
                                        this.List_UDP[i].Send(data, data.Length, List_IP[i], List_Port[i]);

                                    }
                                    if (IsArchive)
                                    {
                                        if ((bool)this.checkBox_Archive.IsChecked)
                                        {
                                            Compositionmat = Archivemat;

                                        }
                                        else
                                        {
                                            Compositionmat = depthMat + Archivemat;

                                        }
                                        Cv2.Resize(Compositionmat, sendmat, new OpenCvSharp.CPlusPlus.Size(sendmat.Width, sendmat.Height));

                                        data = this.sendmat.ToBytes(".jpg");
                                        this.List_UDP[i].Send(data, data.Length, List_IP[i], List_Port[i]);

                                    }
                                }
                                else if((bool)checkBox_Polygon.IsChecked== true && (bool)checkBox_Zanzou.IsChecked == false) 
                                {
                                    if (!IsArchive)
                                    {
                                        Cv2.Resize(Polygon, sendmat, new OpenCvSharp.CPlusPlus.Size(sendmat.Width, sendmat.Height));

                                        data = this.sendmat.ToBytes(".jpg");
                                        this.List_UDP[i].Send(data, data.Length, List_IP[i], List_Port[i]);

                                    }
                                    if (IsArchive)
                                    {

                                        lock (SyncObject)
                                        {
                                            if ((bool)this.checkBox_Archive.IsChecked)
                                            {

                                                Compositionmat = ArchivePolygon;

                                            }
                                            else
                                            {
                                                Compositionmat = Polygon + ArchivePolygon;

                                            }
                                        }
                                        Cv2.Resize(Compositionmat, sendmat, new OpenCvSharp.CPlusPlus.Size(sendmat.Width, sendmat.Height));

                                        data = this.sendmat.ToBytes(".jpg");
                                        this.List_UDP[i].Send(data, data.Length, List_IP[i], List_Port[i]);

                                    }
                                }

                                else if ((bool)checkBox_Zanzou.IsChecked == true && (bool)checkBox_Polygon.IsChecked == false)
                                {
                                    if (!IsArchive)
                                    {
                                        Cv2.Resize(Zanzou, sendmat, new OpenCvSharp.CPlusPlus.Size(sendmat.Width, sendmat.Height));

                                        data = this.sendmat.ToBytes(".jpg");
                                        this.List_UDP[i].Send(data, data.Length, List_IP[i], List_Port[i]);

                                    }
                                    if (IsArchive)
                                    {

                                        if ((bool)this.checkBox_Archive.IsChecked)
                                        {

                                            Compositionmat = ArchiveZanzou;

                                        }
                                        else
                                        {
                                            Compositionmat = Zanzou + ArchiveZanzou;

                                        }
                                        Cv2.Resize(Compositionmat, sendmat, new OpenCvSharp.CPlusPlus.Size(sendmat.Width, sendmat.Height));

                                        data = this.sendmat.ToBytes(".jpg");
                                        this.List_UDP[i].Send(data, data.Length, List_IP[i], List_Port[i]);

                                    }
                                }

                            }
                            //Console.WriteLine("send");
                        }
                    }
                }
            }));

        }

        //エラー検出
        void ErrorPoint(string methodName)
        {
            //Console.WriteLine("Error : " + this.ToString() + ":" + methodName );
            this.message.Content = "Error : " + this.ToString() + ":" + methodName;
        }   
        //終了時の処理
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            //カラーリーダーの終了      
            if (this.colorFrameReader != null)
            {
                this.colorFrameReader.Dispose();
                this.colorFrameReader = null;
            }
            //ディプスリーダーの終了
            if (this.depthFrameReader != null)
            {
                this.depthFrameReader.Dispose();
                this.depthFrameReader = null;
            }
            //キネクトの終了
            if (this.kinect != null)
            {
                this.kinect.Close();
            }
            //Thread
            this.timer.Stop();
            //Archive
            if(this.ReadThread != null)
            {
                this.ReadThread.Abort();

            }
            if(this.DepthThread != null)
            {
                this.DepthThread.Abort();
            }
            if(reader != null)
            {
                reader.Close();
            }
            endthread = true;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox obj = (TextBox)sender;
            if(obj.Text != "")
            {
                try
                {
                    switch (obj.Name)
                    {
                        case "Back": this.space.Back = float.Parse(obj.Text);   ; break;
                        case "Front": this.space.Front = float.Parse(obj.Text); ; break;
                        case "Floor": this.space.Floor = float.Parse(obj.Text); ; break;
                        case "Roof": this.space.Roof = float.Parse(obj.Text);   ; break;
                        case "Left": this.space.Left = float.Parse(obj.Text);   ; break;
                        case "Right": this.space.Right = float.Parse(obj.Text); ; break;

                    }
                }
                catch { }
            }

        }

        
        private void ReadDepth_Click(object sender, RoutedEventArgs e)
        {
            
            if(FilePath != null)
            {
                if(ReadThread != null)
                {
                    ReadThread.Abort();
                }
                if(DepthThread != null)
                {
                    DepthThread.Abort();
                }
                this.kinect.Open();
                this.FpsAd = new FPSAdjuster.FPSAdjuster();
                this.FpsAd.Fps = 30;
                this.FpsAd.Start();

                Isreader = true;
                this.reader = new BinaryReader(File.OpenRead(FilePath));
                this.DepthThread = new Thread(new ThreadStart(this.ReadData));
                this.DepthThread.Start();
               
                this.ReadThread = new Thread(new ThreadStart(this.Archive));
                this.ReadThread.Start();
                IsArchive = true;

            }

        }

        private void ChooseFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.FileName = "";
            ofd.InitialDirectory = @"C:\";
            ofd.FilterIndex = 2;
            ofd.Title = "開くファイルを選択してください";
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.FilePath = ofd.FileName;

            }
        }
        void ReadData()
        {
            //Console.WriteLine("ok");
            while (true)
            {
                FpsAd.Adjust();

                if (Isreader)
                {
                    ////Debug.Log("ok2");
                    //this.datalength = this.reader.ReadInt32();

                    //for (int i = 0; i < datalength; i++)
                    //{
                    //    this.readData[i] = this.reader.ReadUInt16();

                    //}

                    //if (reader.PeekChar() == -1)
                    //{
                    //    //Debug.Log("ok3");
                    //    reader.Close();
                    //    Isreader = false;
                    //    //endthread = true;
                    //}

                    //if (ReadStop)
                    //{
                    //    Isreader = false;
                    //    ReadStop = false;
                    //    //endthread = true;
                    //}
                    ////Console.WriteLine("OK");

                    this.reader.ReadInt32();
                    this.reader.ReadInt32();
                    int mode = this.reader.ReadInt32();
                    //if(mode % 10 == 1)
                    //{
                    //robot
                    this.reader.ReadSingle();
                    this.reader.ReadSingle();
                    this.reader.ReadSingle();
                    this.reader.ReadSingle();
                    this.reader.ReadSingle();
                    this.reader.ReadSingle();

                    //}
                    //if (mode % 10 == 1)
                    //{
                    //Shadow
                    for (int i = 0; i < 512 * 424; i++)
                    {
                        this.readData[i] = this.reader.ReadUInt16();

                    }
                    //}
                    //if (mode / 100 == 1)
                    //{
                    //LRF
                    int human = this.reader.ReadInt32();
                    for (int i = 0; i < human; i++)
                    {
                        this.reader.ReadInt32();
                        this.reader.ReadSingle();
                        this.reader.ReadSingle();
                        this.reader.ReadSingle();
                    }

                    //}

                }
                else
                {
                    if (reader != null)
                    {
                        reader.Close();

                    }

                    break;
                }

            }
            //Console.WriteLine("in");
            
            if (reader != null)
            {
                reader.Close();
            }
            if (FilePath != null)
            {
                FilePath = null;
            }

            if (DepthThread != null)
            {
                DepthThread.Abort();
            }
            //if(ReadThread != null)
            //{
            //    ReadThread.Abort();
            //}

            //if (this.readData.Length != 0)
            //{
                
            //    this.readData = new ushort[512 * 424];

            //}

        }

        private void ReadStopButton_Click(object sender, RoutedEventArgs e)
        {
            ReadStop = true;
        }

        void Archive()
        {
            //Console.WriteLine("a");
            while (true)
            {
                this.kinect.CoordinateMapper.MapDepthFrameToCameraSpace(this.readData, this.cameraSpacePointsArchive);

                this.ArchiveDepthToMat();
                Console.WriteLine("a");

                if (endthread)
                {
                    break;
                }
            }

        }
        void ArchiveDepthToMat()
        {
            int Archivechannel = this.Archivemat.Channels();
            unsafe
            {
                byte* ArchivematPtr = this.Archivemat.DataPointer;
                //Console.WriteLine(cameraSpacePointsArchive.Length);
                for (int i = 0; i < this.cameraSpacePointsArchive.Length; i++)
                {
                    //条件で絞る
                    if (this.cameraSpacePointsArchive[i].Z < this.space.Back &&
                        this.cameraSpacePointsArchive[i].Z > this.space.Front &&
                        this.cameraSpacePointsArchive[i].Y > this.space.Floor &&
                        this.cameraSpacePointsArchive[i].Y < this.space.Roof &&
                        this.cameraSpacePointsArchive[i].X > this.space.Left &&
                        this.cameraSpacePointsArchive[i].X < this.space.Right)
                    {
                        //条件クリア（人）
                        for (int j = 0; j < Archivechannel; j++)
                        {
                            *(ArchivematPtr + i * Archivechannel + j) = 255;
                        }

                    }
                    else
                    {
                        //人以外
                        for (int j = 0; j < Archivechannel; j++)
                        {
                            *(ArchivematPtr + i * Archivechannel + j) = 0;
                        }
                    }
                }
            }

            Cv2.ImShow("label", Archivemat);
            if (bool_label)
            {
                Labeling();
            }

            #region
            checkBox_Polygon.Dispatcher.BeginInvoke(new Action(() =>
            {

                if ((bool)checkBox_Polygon.IsChecked)
                {
                    IsPolygon = true;
                }
                else
                {
                    IsPolygon = false;
                }


            }));
            checkBox_Zanzou.Dispatcher.BeginInvoke(new Action(() =>
            {

                if ((bool)checkBox_Zanzou.IsChecked)
                {
                    IsZanzou = true;
                }
                else
                {
                    IsZanzou = false;
                }

            }));
            if (IsPolygon)
            {
                try
                {
                    this.List_ArchiveContours.Clear();
                    Cv2.CvtColor(Archivemat, Archivegrayimage, OpenCvSharp.ColorConversion.BgrToGray);
                    lock (SyncObject)
                    {
                        ArchivePolygon = new Mat(424, 512, MatType.CV_8UC3);
                        OpenCvSharp.CPlusPlus.Point[][] contour;//= grayimage.FindContoursAsArray(OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxSimple);
                        OpenCvSharp.CPlusPlus.HiearchyIndex[] hierarchy;

                        Cv2.FindContours(Archivegrayimage, out contour, out hierarchy, OpenCvSharp.ContourRetrieval.External, OpenCvSharp.ContourChain.ApproxNone);

                        List<OpenCvSharp.CPlusPlus.Point> CvPoints = new List<OpenCvSharp.CPlusPlus.Point>();

                        RedSlider.Dispatcher.BeginInvoke(new Action(() =>
                        {

                            red = (int)RedSlider.Value;

                        }));

                        GreenSlider.Dispatcher.BeginInvoke(new Action(() =>
                        {

                            green = (int)GreenSlider.Value;

                        }));

                        BlueSlider.Dispatcher.BeginInvoke(new Action(() =>
                        {

                            blue = (int)BlueSlider.Value;

                        }));

                        for (int i = 0; i < contour.Length; i++)
                        {
                            CvPoints.Clear();

                            if (Cv2.ContourArea(contour[i]) > 1000)
                            {

                                for (int j = 0; j < contour[i].Length; j += this.sharpness)
                                {

                                    CvPoints.Add(contour[i][j]);
                                }

                                this.List_ArchiveContours.Add(CvPoints);
                                //Cv2.FillConvexPoly(dst, CvPoints, Scalar.Yellow,  OpenCvSharp.LineType.Link4, 0);
                                Cv2.DrawContours(ArchivePolygon, this.List_ArchiveContours, 0, Scalar.FromRgb(red, green, blue), -1, OpenCvSharp.LineType.Link8);

                            }

                        }
                    }
                    //GC.Collect();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            if (IsZanzou)
            {
                try
                {
                    Cv2.CvtColor(Archivemat, bufimage, OpenCvSharp.ColorConversion.BgrToGray);

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

                    //    //////

                    Cv2.Dilate(bufimage, bufimage, m_element);
                    Cv2.Dilate(bufimage, bufimage, m_element);

                    Cv2.Erode(bufimage, bufimage, m_element);
                    Cv2.Erode(bufimage, bufimage, m_element);


                    ////cvFindContoursを用いた輪郭抽出*****************************
                    Mat tmp_bufImage_next;
                    Mat tmp_bufImage_next3;

                    //TODO:移動可
                    tmp_bufImage_next = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC1, new Scalar(0));
                    tmp_bufImage_next3 = new Mat(new OpenCvSharp.CPlusPlus.Size(_w, _h), MatType.CV_8UC1, new Scalar(0));

                    bufimage.CopyTo(tmp_bufImage_next);

                    OpenCvSharp.CPlusPlus.Point[][] contours;
                    OpenCvSharp.CPlusPlus.HiearchyIndex[] hierarchy;

                    /// Find contours
                    Cv2.FindContours(tmp_bufImage_next, out contours, out hierarchy, OpenCvSharp.ContourRetrieval.Tree, OpenCvSharp.ContourChain.ApproxNone);

                    /// Draw contours
                    for (int i = 0; i < contours.Length; i++)
                    {
                        Scalar color = new Scalar(255);
                        //Cv2.DrawContours(tmp_bufImage_next3, contours, i, color, 2, OpenCvSharp.LineType.Link8, hierarchy, 0);
                        Cv2.FillPoly(tmp_bufImage_next3, contours, color);
                    }


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

                    RedSlider.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        red = (int)RedSlider.Value;

                    }));

                    GreenSlider.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        green = (int)GreenSlider.Value;

                    }));

                    BlueSlider.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        blue = (int)BlueSlider.Value;

                    }));


                    RedSlider_Copy.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        outred = (int)RedSlider_Copy.Value;

                    }));

                    GreenSlider_Copy.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        outgreen = (int)GreenSlider_Copy.Value;

                    }));

                    BlueSlider_Copy.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        outblue = (int)BlueSlider_Copy.Value;

                    }));

                    Mat tmpColorBuffer2 = new Mat(new OpenCvSharp.CPlusPlus.Size(bufimage.Width, bufimage.Height), MatType.CV_8UC3, new Scalar(255, 255, 255));
                    outerColorBuffer2.SetTo(new Scalar(outred, outgreen, outblue));
                    Cv2.CvtColor(outerGrayBuffer2, tmpColorBuffer2, OpenCvSharp.ColorConversion.GrayToBgr);
                    Cv2.Multiply(outerColorBuffer2, tmpColorBuffer2, outerColorBuffer2, 1.0 / 255.0);
                    innerColorBuffer2.SetTo(new Scalar(red, green, blue));
                    Cv2.CvtColor(innerGrayBuffer2, tmpColorBuffer2, OpenCvSharp.ColorConversion.GrayToBgr);
                    Cv2.Multiply(innerColorBuffer2, tmpColorBuffer2, innerColorBuffer2, 1.0 / 255.0);

                    outerColorBuffer2 -= innerColorBuffer2;
                    outerColorBuffer2.GaussianBlur(new OpenCvSharp.CPlusPlus.Size(3, 3), 3).CopyTo(ArchiveZanzou);

                    GC.Collect();
                }

                
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.TargetSite);
                }
               
            }
            #endregion
        }

        private void labeling_Click(object sender, RoutedEventArgs e)
        {
            bool_label = true;
        }

        void Labeling()
        {

            labelimage = (IplImage)Archivemat;
            //CvWindow.ShowImages(labelimage);


            Cv.CvtColor(labelimage, labelgrayimage, ColorConversion.BgrToGray);
            CvBlobs blobs = new CvBlobs();
            blobs.Label(labelgrayimage);
            blobs.FilterByArea(100, int.MaxValue);
            IplImage imgrender = new IplImage(labelimage.Size, BitDepth.U8, 3);
            blobs.RenderBlobs(labelgrayimage, imgrender);
            //CvWindow.ShowImages(imgrender);
            Cv.ShowImage("label", imgrender);
        }



    }
}
