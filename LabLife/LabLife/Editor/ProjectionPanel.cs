﻿//ガベージコレクタを強制呼び出し
//#define USE_GC

using LabLife.Contorols;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using OpenCvSharp.CPlusPlus;
using LabLife.Processer;

namespace LabLife.Editor
{
    public class ProjectionPanel : ADefaultPanel
    {
        private int m_ProjectionPanelId;
        private Image Image_Main = new Image();
        private TextBlock TextBlock_Header = new TextBlock();
        private SliderAndTextControl SliderAndTextControl_margin = new SliderAndTextControl();

        private List<Transporter> m_SenderList = new List<Transporter>();

        public int ImageWidth
        {
            get
            {
                return this.m_ProjectionImageMatrix.Width;
            }
        }



        public int ImageHeight
        {
            get
            {
                return this.m_ProjectionImageMatrix.Height;
            }
        }

        private WriteableBitmap m_WritableBitmap;

        public OpenCvSharp.CPlusPlus.Mat m_ProjectionImageMatrix;
        public byte[] m_data;

        public ProjectionPanel(int id)
        {
            this.m_ProjectionPanelId = id;
            this.TitleName = "Projection Panel " + this.m_ProjectionPanelId.ToString();
        }
        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            this.TextBlock_Header.Text = this.ToString();
            this.AddContent(this.TextBlock_Header, Dock.Top);
            this.AddContent(this.SliderAndTextControl_margin, Dock.Top);
            this.AddContent(this.Image_Main, Dock.Top);

            this.SliderAndTextControl_margin.Slider_Main.Maximum = 200;
            this.SliderAndTextControl_margin.Slider_Main.Minimum = 0;
            this.SliderAndTextControl_margin.Slider_Main.ValueChanged += Slider_Main_ValueChanged;
            this.SliderAndTextControl_margin.TextBlock_Title.Text = "Margin";
            this.m_data = new byte[256 * 256 * 3];
            this.m_ProjectionImageMatrix = new OpenCvSharp.CPlusPlus.Mat(256, 256, OpenCvSharp.CPlusPlus.MatType.CV_8UC3, new Scalar(0, 0, 0));
            this.m_WritableBitmap = new WriteableBitmap(256, 256, 96, 96, PixelFormats.Bgr24, null);
            this.Image_Main.Source = this.m_WritableBitmap;
        }

        private void Slider_Main_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.SliderAndTextControl_margin.Slider_Main.Value = ((int)(this.SliderAndTextControl_margin.Slider_Main.Value));
            this.Image_Main.Margin = new Thickness(this.SliderAndTextControl_margin.Slider_Main.Value);
        }

        /// <summary>
        /// transporterから呼ばれるが，自動的にすべてのUpdateImageがきたときのみ呼ばれる
        /// </summary>
        /// <param name="Sender"></param>
        public void UpdateImage(Transporter Sender)
        {
            if (m_SenderList == null)
            {
                return;
            }
            if (Sender != this.m_SenderList.Last())
            {
                return;
            }
            this.m_WritableBitmap.WritePixels(new Int32Rect(0, 0, 256, 256), m_ProjectionImageMatrix.Data, 256 * 256 * 3, 256 * 3);
            this.m_ProjectionImageMatrix.Dispose();
#if USE_GC
            GC.Collect();
#endif
        }
        public void InitImage(Transporter Sender, Mat mat)
        {
            if (m_SenderList == null)
            {
                return;
            }
            if (Sender != this.m_SenderList[0])
            {
                return;
            }

            this.m_ProjectionImageMatrix = mat.Clone();
        }
        Scalar black = new Scalar(0, 0, 0);

        public void AddSenderList(Transporter Sender)
        {
            this.m_SenderList.Add(Sender);
        }
        public void RemoveSenderList(Transporter Sender)
        {
            this.m_SenderList.Remove(Sender);
        }

        public override void Close(object sender, RoutedEventArgs e)
        {
            base.Close(sender, e);
            this.m_ProjectionImageMatrix.Dispose();
        }
        public override string ToString()
        {
            return this.TitleName;
        }
    }
}
