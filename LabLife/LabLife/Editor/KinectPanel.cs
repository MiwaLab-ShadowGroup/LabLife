using LabLife.Contorols;
using LabLife.Data;
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
using System.Windows.Shapes;

namespace LabLife.Editor
{
    public class KinectPanel : AImageResourcePanel
    {

        LLCheckBox KinectButton = new LLCheckBox();
        TextBlock Bonestatus = new TextBlock();
        Canvas canvas1 = new Canvas();

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

        //colorimage関連
        ColorImageFormat colorImageFormat;
        ColorFrameReader colorFrameReader;
        FrameDescription colorFrameDescription;
        WriteableBitmap colorimage;
        WriteableBitmap calibImg;
        Int32Rect bitmapRect;
        int bitmapStride;
        byte[] colors;
        int imageWidth;
        int imageHeigt;

        //骨格情報
        BodyFrameReader bodyFrameReader;
        Body[] bodies;
        int NumberofPlayer;

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

            //colorimage
            this.colorImageFormat = ColorImageFormat.Bgra;
            this.colorFrameDescription = this.kinect.ColorFrameSource.CreateFrameDescription(this.colorImageFormat);
            this.colorFrameReader = this.kinect.ColorFrameSource.OpenReader();
            this.colorFrameReader.FrameArrived += ColorFrame_Arrived;
            this.colors = new byte[this.colorFrameDescription.Width
                                           * this.colorFrameDescription.Height
                                           * this.colorFrameDescription.BytesPerPixel];
            this.imageWidth = this.colorFrameDescription.Width;
            this.imageHeigt = this.colorFrameDescription.Height;
            this.colorimage = new WriteableBitmap(this.colorFrameDescription.Width, this.colorFrameDescription.Height, 96, 96, PixelFormats.Bgr32, null);
            this.calibImg = new WriteableBitmap(this.colorFrameDescription.Width, this.colorFrameDescription.Height, 96, 96, PixelFormats.Bgr32, null);
            this.bitmapRect = new Int32Rect(0, 0, this.colorFrameDescription.Width, this.colorFrameDescription.Height);
            this.bitmapStride = this.colorFrameDescription.Width * (int)this.colorFrameDescription.BytesPerPixel;

            //bone
            this.bodyFrameReader = this.kinect.BodyFrameSource.OpenReader();
            this.bodyFrameReader.FrameArrived += BodyFrame_Arrived;
            this.bodies = new Body[this.kinect.BodyFrameSource.BodyCount]; //bodycountに骨格情報の数

        }

        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            var p = new System.Windows.Controls.Image();
            var q = new System.Windows.Controls.Image();
            var r = new System.Windows.Controls.Image();

            Border b = new Border();
            b.Style = (Style)App.Current.Resources["Border_Default"];
            b.Child = this.KinectButton;
            base.AddContent(b, Dock.Top);
            KinectButton.Content = "Kinect Start";
            KinectButton.Click += KinectButton_Click;

            base.AddContent(Bonestatus, Dock.Top);

            base.SetImageToGridChildren(p);

            p.Source = bodyIndexColorImage;
            base.SetImageToGridChildren(q);
            q.Source = depthImage;
            base.SetImageToGridChildren(r);
            r.Source = colorimage;
            this.AddContent(base.Grid_Image, Dock.Top);

            this.AddContent()
        }

        //close処理
        public override void Close(object sender, RoutedEventArgs e)
        {
            this.KinectClose();
            base.Close(sender, e);
            
        }

        //Kinect
        #region
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
        #endregion

        //bodyindex
        #region
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
        #endregion

        //Depth
        #region
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
                    
                }

                this.depthImage.WritePixels(depthRect, depthBuffer, depthStride, 0);

            }
        }
        #endregion

        //ColorImage
        #region
        void ColorFrame_Arrived(object sender, ColorFrameArrivedEventArgs e)
        {
            try
            {
                ColorFrame colorFrame = e.FrameReference.AcquireFrame();
                //フレームがなければ終了、あれば格納
                if (colorFrame == null) return;
                colorFrame.CopyConvertedFrameDataToArray(this.colors, this.colorImageFormat);
                //表示
                this.colorimage.WritePixels(this.bitmapRect, this.colors, this.bitmapStride, 0);
                

                //破棄
                colorFrame.Dispose();
            }
            catch
            {
                General.Log(this, "ColorImageError");
            }

        }
        #endregion

        //Bone
        void BodyFrame_Arrived(object sender, BodyFrameArrivedEventArgs e)
        {
            try
            {
                // キャンバスをクリアする
                canvas1.Children.Clear();

                BodyFrame bodyFrame = e.FrameReference.AcquireFrame();
                if (bodyFrame == null) return;
                bodyFrame.GetAndRefreshBodyData(this.bodies);
                

                foreach (var body in bodies.Where(b => b.IsTracked))
                {
                    foreach (Joint joint in body.Joints.Values)
                    {
                        
                        Bonestatus.Text = "X=" + body.Joints[JointType.SpineBase].Position.X + "Y=" + body.Joints[JointType.SpineBase].Position.Y + "Z=" + body.Joints[JointType.SpineBase].Position.Z;

                        //CameraSpacePoint jointPosition = joint.Position;

                        //ColorSpacePoint Colorpoint = new ColorSpacePoint();
                        //kinect.CoordinateMapper.MapCameraPointsToColorSpace(joint.Position, Colorpoint);
                        
                    }

                    Ellipse ellipse = new Ellipse
                    {
                        Fill = System.Windows.Media.Brushes.Red,
                        Width = 15,
                        Height = 15
                    };

                    Canvas.SetLeft(ellipse, body.Joints[JointType.SpineBase].Position.X);
                    Canvas.SetTop(ellipse, body.Joints[JointType.SpineBase].Position.Y);

                    canvas1.Children.Add(ellipse);

                }
                //破棄
                bodyFrame.Dispose();
            }
            catch
            {
                General.Log(this, "BoneError");
            }

        }


    }
}
