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
        Button KinectButton = new Button();

        KinectSensor kinect;
        BodyIndexFrameReader bodyIndexFrameReader;
        FrameDescription bodyIndexFrameDesc;

        byte[] bodyIndexBuffer;

        WriteableBitmap bodyIndexColorImage;
        

        Int32Rect bodyIndexColorRect;
        int bodyIndexColorStride;
        int bodyIndexColorBytesPerPixel = 4;
        byte[] bodyIndexColorBuffer;
        System.Windows.Media.Color[] bodyIndexColors;

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
        }

        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            var p = new KinectPanelControl();

            base.AddContent(KinectButton, Dock.Top);
            KinectButton.Content = "Kinect Start";
            KinectButton.Click += KinectButton_Click;

            base.AddContent(p, Dock.Bottom);
            p.Image_Kinect.Source = bodyIndexColorImage;

            
        }



        public void KinectButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.KinectButton.Content.ToString() == "Kinect Start")
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
            else if (this.KinectButton.Content.ToString() == "Kinect Stop")
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

                bodyIndexFrame.Dispose();
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


    }
}
