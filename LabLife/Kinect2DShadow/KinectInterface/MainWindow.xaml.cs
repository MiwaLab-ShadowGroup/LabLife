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

namespace Kinect2DShadow
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        //宣言
        #region
            //a
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
        UdpClient udpClient;
        DispatcherTimer timer;
        #endregion

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

            //震度情報
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
            this.timer.Tick += new EventHandler(this.SendImage);
            this.timer.Start();
            
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

            this.udpClient = new UdpClient();

            this.message.Content = "ConnectUDP";
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
                        for(int j= 0; j < channel ; j ++)
                        {
                            *(matPtr + i * channel + j) = 255;
                        }
                       
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
                if (this.udpClient != null)
                {
                    byte[] data;

                    if ((bool)this.radioButton_BI.IsChecked)
                    {
                        data = this.bodyIndexMat.ToBytes(".jpg");
                        this.udpClient.Send(data, data.Length, this.UDPip.Text, int.Parse(this.UDPport.Text));
                    }
                    else if ((bool)this.radioButton_depth.IsChecked)
                    {
                        data = this.depthMat.ToBytes(".jpg");
                        this.udpClient.Send(data, data.Length, this.UDPip.Text, int.Parse(this.UDPport.Text));
                    }
                    //Console.WriteLine("send");
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

       
    }
}
