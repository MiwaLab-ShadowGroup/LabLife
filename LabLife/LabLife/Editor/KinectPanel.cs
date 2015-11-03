using LabLife.Contorols;
using Microsoft.Kinect;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LabLife.Editor
{
    public class KinectPanel : DefaultPanel
    {

        LLCheckBox KinectButton = new LLCheckBox();
        
        KinectSensor kinect;

        //bodyindex関連
        BodyIndexFrameReader bodyIndexFrameReader;
        FrameDescription bodyIndexFrameDesc;
        byte[] bodyIndexBuffer;
        WriteableBitmap bodyIndexColorImage;
        
        Int32Rect bodyIndexColorRect;
        int bodyIndexColorStride;
        int bodyIndexColorBytesPerPixel = 4;
        byte[] bodyIndexColorBuffer;
        System.Windows.Media.Color[] bodyIndexColors;

        //depth関連
        DepthFrameReader depthFrameReader;
        FrameDescription depthFrameDescription;
        Int32Rect depthRect;
        int depthStride;
        ushort[] depthBuffer;
        int depthImageWidth;
        int depthImageHeight;
        WriteableBitmap depthImage;

        private int[] depthBitdata;


        public KinectPanel()
        {
            base.TitleName = "Kinect Condition";
            kinect = KinectSensor.GetDefault();

            //bodyindex関連
            this.bodyIndexFrameDesc = kinect.DepthFrameSource.FrameDescription;
            this.bodyIndexBuffer = new byte[bodyIndexFrameDesc.LengthInPixels];
            this.bodyIndexColorImage = new WriteableBitmap(bodyIndexFrameDesc.Width, bodyIndexFrameDesc.Height,
                    96, 96, PixelFormats.Bgra32, null);
            this.bodyIndexColorRect = new Int32Rect(0, 0, bodyIndexFrameDesc.Width, bodyIndexFrameDesc.Height);
            this.bodyIndexColorStride = (int)(bodyIndexFrameDesc.Width * bodyIndexColorBytesPerPixel);
            this.bodyIndexColorBuffer = new byte[bodyIndexFrameDesc.LengthInPixels * bodyIndexColorBytesPerPixel];

            bodyIndexColors = new System.Windows.Media.Color[]{
                    Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Pink, Colors.Purple,
                };
            bodyIndexFrameReader = kinect.BodyIndexFrameSource.OpenReader();
            bodyIndexFrameReader.FrameArrived += bodyIndexFrameReader_FrameArrived;

            //depth関連
            this.depthFrameReader = this.kinect.DepthFrameSource.OpenReader();
            this.depthFrameReader.FrameArrived += DepthFrame_Arrived;
            this.depthFrameDescription = this.kinect.DepthFrameSource.FrameDescription;
            this.depthBuffer = new ushort[this.depthFrameDescription.LengthInPixels];
            this.depthImageWidth = this.depthFrameDescription.Width;
            this.depthImageHeight = this.depthFrameDescription.Height;
            this.depthImage = new WriteableBitmap(depthFrameDescription.Width, depthFrameDescription.Height, 96, 96, PixelFormats.Gray16, null);

            this.depthRect = new Int32Rect(0, 0, depthFrameDescription.Width, depthFrameDescription.Height);
            this.depthStride = (int)(depthFrameDescription.Width * depthFrameDescription.BytesPerPixel);
        }

        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            var p = new KinectPanelControl();
            var q = new KinectPanelControl();

            Border b = new Border();
            b.Style = (Style)App.Current.Resources["Border_Default"];
            b.Child = this.KinectButton;
            base.AddContent(b, Dock.Top);
            KinectButton.Content = "Kinect Start";
            KinectButton.Click += KinectButton_Click;


            base.AddContent(p, Dock.Left);
            p.Image_Kinect.Source = bodyIndexColorImage;
            base.AddContent(q, Dock.Left);
            q.Image_Kinect.Source = depthImage;

        }

        //close処理
        public override void Close(object sender, RoutedEventArgs e)
        {
            this.KinectClose();
            base.Close(sender, e);
            
        }
        

        public void KinectButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.kinect == null) this.kinect = KinectSensor.GetDefault();
            if (!this.kinect.IsOpen)
            {
                this.KinectStart();
            }
            else
            {

                this.KinectClose();
            }

        }

        private void KinectStart()
        {
            //kinect Start
            try
            {
                this.kinect.Open();
                this.KinectButton.Content = "Kinect Stop";
                if (this.kinect == null) this.kinect = KinectSensor.GetDefault();
            }
            catch
            {
                Console.WriteLine("error:kinectstart");

            }
        }

        private void KinectClose()
        {
            //kinect Stop
            try
            {
                if (this.kinect != null) this.kinect.Close();
                this.KinectButton.Content = "Kinect Start";
            }
            catch
            {
                Console.WriteLine("error:kinectstop");

            }
        }

        void bodyIndexFrameReader_FrameArrived(object sender, BodyIndexFrameArrivedEventArgs e)
        {

            UpdateBodyIndexFrame(e);
            DrawBodyIndexFrame();

        }

        private void UpdateBodyIndexFrame(BodyIndexFrameArrivedEventArgs e)
        {
            using (var bodyIndexFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyIndexFrame == null)
                {
                    return;
                }

                // ボディインデックスデータを取得する
                bodyIndexFrame.CopyFrameDataToArray(bodyIndexBuffer);

                //bodyIndexFrame.Dispose();
            }
        }

        private void DrawBodyIndexFrame()
        {
            try
            {
                // ボディインデックスデータをBGRAデータに変換する

                for (int i = 0; i < bodyIndexBuffer.Length; i++)
                {
                    var index = bodyIndexBuffer[i];
                    var colorIndex = i * 4;

                    if (index != 255 )
                    {

                        var color = bodyIndexColors[index];
                        bodyIndexColorBuffer[colorIndex + 0] = color.B;
                        bodyIndexColorBuffer[colorIndex + 1] = color.G;
                        bodyIndexColorBuffer[colorIndex + 2] = color.R;
                        bodyIndexColorBuffer[colorIndex + 3] = 255;

                    }
                    if (index == 255)
                    {
                        bodyIndexColorBuffer[colorIndex + 0] = 255;
                        bodyIndexColorBuffer[colorIndex + 1] = 255;
                        bodyIndexColorBuffer[colorIndex + 2] = 255;
                        bodyIndexColorBuffer[colorIndex + 3] = 255;
                    }

                }

                // ビットマップにする
                this.bodyIndexColorImage.WritePixels(bodyIndexColorRect, bodyIndexColorBuffer, bodyIndexColorStride, 0);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        void DepthFrame_Arrived(object sender, DepthFrameArrivedEventArgs e)
        {
            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {

                //フレームがなければ終了、あれば格納
                if (depthFrame == null) return;

                if (depthBitdata == null)
                {
                    depthBitdata = new int[depthBuffer.Length];
                }
                depthFrame.CopyFrameDataToArray(this.depthBuffer);

                //取得範囲指定するときはコメントアウト解除
                //var max = 3500;
                //var min = 1000;

                for (int i = 0; i < depthBuffer.Length; i++)
                {
                    //if (max > depthBuffer[i] &&
                    //    min < depthBuffer[i])
                    //{
                        depthBuffer[i] = (ushort)(depthBuffer[i] * 65535 / 8000);

                    //}
                    //else
                    //{
                    //    depthBuffer[i] = ushort.MinValue;
                    //}
                    //depthBuffer[i] = (ushort)(depthBuffer[i] * 65535 / 8000);
                }

                this.depthImage.WritePixels(depthRect, depthBuffer, depthStride, 0);

                
            }
        }

    }
}
