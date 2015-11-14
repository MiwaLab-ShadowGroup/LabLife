using LabLife.Contorols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using LabLife.Data;

namespace LabLife.Editor
{
    public class ImageReceiverPanel : AImageResourcePanel
    {
        private int m_ImageReceiveID;
        private Image Image_Main = new Image();
        private TextBlock TextBlock_Header = new TextBlock();
        private SliderAndTextControl SliderAndTextControl_margin = new SliderAndTextControl();


        private MemoryStream m_receivedMemory;
        private UdpClient m_receiver;
        private BitmapSource bitmapsorce;
        private WriteableBitmap m_WritableBitmap;
        private JpegBitmapDecoder jpegDec;
        private byte[] m_data;
        private byte[] Received_data;
        private Mat m_mat;

        public int ImageReceiveID
        {
            get
            {
                return this.m_ImageReceiveID;
            }
        }

        public int OpenPortNum
        {
            get
            {
                return ImageReceiverHostPanel.StartPortNum + this.m_ImageReceiveID;
            }
        }


        public ImageReceiverPanel(int id)
        {
            this.m_ImageReceiveID = id;
            this.TitleName = "Image Receiver" + ImageReceiveID.ToString();
        }
        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            this.TextBlock_Header.Text = this.ToString();
            this.AddContent(this.TextBlock_Header, Dock.Top);
            this.AddContent(this.SliderAndTextControl_margin, Dock.Top);
            base.SetImageToGridChildren(this.Image_Main);
            this.AddContent(base.Grid_Image, Dock.Top);

            this.SliderAndTextControl_margin.Slider_Main.Maximum = 200;
            this.SliderAndTextControl_margin.Slider_Main.Minimum = 0;
            this.SliderAndTextControl_margin.Slider_Main.ValueChanged += Slider_Main_ValueChanged;
            this.SliderAndTextControl_margin.TextBlock_Title.Text = "Margin";
            //Uri test = new Uri(@"C:\workspace\LabLife\LabLife\LabLife\bin\Debug\kakikakefusou.JPG");
            //this.Image_Main.Source = new BitmapImage(test);

            this.m_receiver = new UdpClient(this.OpenPortNum);
            this.m_receiver.BeginReceive(ReceiveCallBack, this.m_receiver);
        }

        private void Slider_Main_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.SliderAndTextControl_margin.Slider_Main.Value = ((int)(this.SliderAndTextControl_margin.Slider_Main.Value));
            this.Image_Main.Margin = new Thickness(this.SliderAndTextControl_margin.Slider_Main.Value);
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                this.Received_data = this.m_receiver.EndReceive(ar, ref remoteEP);
                this.m_receivedMemory = new MemoryStream(this.Received_data);
                this.updateImage();
                this.m_receivedMemory.Dispose();
                this.m_receiver.BeginReceive(ReceiveCallBack, this.m_receiver);
            }
            catch (SocketException)
            {
                General.Log(this, "close");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void updateImage()
        {

            var p = this.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    //デコード
                    this.jpegDec = new JpegBitmapDecoder(this.m_receivedMemory, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.None);
                    this.bitmapsorce = this.jpegDec.Frames[0];

                    //初期化
                    if (m_WritableBitmap == null)
                    {
                        this.m_WritableBitmap = new WriteableBitmap(this.bitmapsorce);
                        this.m_data = new byte[this.bitmapsorce.PixelWidth * this.bitmapsorce.PixelHeight * this.bitmapsorce.Format.BitsPerPixel / 8];
                        this.Image_Main.Source = this.m_WritableBitmap;


                    }

                    //イベント発生・送信
                    this.bitmapsorce.CopyPixels(this.m_data, this.bitmapsorce.PixelWidth * this.bitmapsorce.Format.BitsPerPixel / 8, 0);
                    this.m_mat = new Mat(this.bitmapsorce.PixelHeight, this.bitmapsorce.PixelWidth, MatType.CV_8UC3, this.m_data);
                    var e = new ImageFrameArrivedEventArgs(new Mat[] { this.m_mat });
                    OnImageFrameArrived(e);

                    //画像の更新
                    this.m_WritableBitmap.WritePixels(new Int32Rect(0, 0, this.m_WritableBitmap.PixelWidth, this.m_WritableBitmap.PixelHeight), this.m_data, this.bitmapsorce.PixelWidth * this.bitmapsorce.Format.BitsPerPixel / 8, 0);

                    //最終処理
                    this.bitmapsorce = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }));

            p.Wait();
        }

        public override void Close(object sender, RoutedEventArgs e)
        {
            base.Close(sender, e);
            this.m_receiver.Close();
        }
        public override string ToString()
        {
            return this.TitleName + "\n"
                + "\tPort : " + this.OpenPortNum.ToString();
        }
    }
}
