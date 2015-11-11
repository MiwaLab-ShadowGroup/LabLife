//ガベージコレクタを強制呼び出し
#define USE_GC

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
using LabLife.Windows;

namespace LabLife.Editor
{
    public class ProjectionPanel : ADefaultPanel
    {
        private int m_ProjectionPanelId;
        private Image Image_Main = new Image();
        public Grid Grid_Image = new Grid();
        private TextBlock TextBlock_Header = new TextBlock();
        private LLCheckBox LLCheckBox_ProjectorWindow = new LLCheckBox();
        private SliderAndTextControl SliderAndTextControl_margin = new SliderAndTextControl();

        private List<Transporter> m_SenderList = new List<Transporter>();

        public ProjectorWindow m_ProjectorWindow = new ProjectorWindow();

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
            this.AddContent(this.LLCheckBox_ProjectorWindow, Dock.Top);
            this.AddContent(this.Grid_Image, Dock.Top);

            this.Grid_Image.Children.Add(Image_Main);

            this.LLCheckBox_ProjectorWindow.Content = "Window";
            this.LLCheckBox_ProjectorWindow.Click += LLCheckBox_ProjectorWindow_Click;

            this.SliderAndTextControl_margin.Slider_Main.Maximum = 200;
            this.SliderAndTextControl_margin.Slider_Main.Minimum = 0;
            this.SliderAndTextControl_margin.Slider_Main.ValueChanged += Slider_Main_ValueChanged;
            this.SliderAndTextControl_margin.TextBlock_Title.Text = "Margin";
            this.m_data = new byte[256 * 256 * 3];
            this.m_ProjectionImageMatrix = new OpenCvSharp.CPlusPlus.Mat(256, 256, OpenCvSharp.CPlusPlus.MatType.CV_8UC3, new Scalar(0, 0, 0));
            this.Image_Main.Source = this.m_WritableBitmap;
        }

        private void LLCheckBox_ProjectorWindow_Click(object sender, RoutedEventArgs e)
        {
            if (this.LLCheckBox_ProjectorWindow.IsChecked)
            {
                this.m_ProjectorWindow.Image_Project.Source = this.m_WritableBitmap;
                this.m_ProjectorWindow.Show();
            }
            else
            {
                this.m_ProjectorWindow.Hide();
            }
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
            if (this.m_WritableBitmap == null)
            {
                this.m_WritableBitmap = new WriteableBitmap(m_ProjectionImageMatrix.Width, m_ProjectionImageMatrix.Height, 96, 96, PixelFormats.Bgr24, null);
                this.Image_Main.Source = this.m_WritableBitmap;
                this.m_ProjectorWindow.Image_Project.Source = this.m_WritableBitmap;
                this.Grid_Image.Width = m_ProjectionImageMatrix.Width;
                this.Grid_Image.Height = m_ProjectionImageMatrix.Height;
            }
            this.m_WritableBitmap.WritePixels(new Int32Rect(0, 0, m_ProjectionImageMatrix.Width, m_ProjectionImageMatrix.Height), m_ProjectionImageMatrix.Data, m_ProjectionImageMatrix.Width * m_ProjectionImageMatrix.Height * m_ProjectionImageMatrix.Type().Channels, m_ProjectionImageMatrix.Width * m_ProjectionImageMatrix.Type().Channels);
            this.m_ProjectionImageMatrix.Dispose();
#if USE_GC
            GC.Collect();
#endif
        }

        public void UpdateImageByThumb()
        {
            if (this.m_WritableBitmap == null)
            {
                this.m_WritableBitmap = new WriteableBitmap(m_ProjectionImageMatrix.Width, m_ProjectionImageMatrix.Height, 96, 96, PixelFormats.Bgr24, null);
                this.Image_Main.Source = this.m_WritableBitmap;
                this.m_ProjectorWindow.Image_Project.Source = this.m_WritableBitmap;
                this.Grid_Image.Width = m_ProjectionImageMatrix.Width;
                this.Grid_Image.Height = m_ProjectionImageMatrix.Height;
            }
            this.m_WritableBitmap.WritePixels(new Int32Rect(0, 0, m_ProjectionImageMatrix.Width, m_ProjectionImageMatrix.Height), m_ProjectionImageMatrix.Data, m_ProjectionImageMatrix.Width * m_ProjectionImageMatrix.Height * m_ProjectionImageMatrix.Type().Channels, m_ProjectionImageMatrix.Width * m_ProjectionImageMatrix.Type().Channels);
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
            this.m_ProjectionImageMatrix.SetTo(black);
        }
        Scalar black = new Scalar(0, 0, 0);

        public void InitImageByThumb(Mat mat)
        {
            this.m_ProjectionImageMatrix = mat.Clone();
            this.m_ProjectionImageMatrix.SetTo(black);
        }

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
            if (this.m_ProjectorWindow != null)
            {
                this.m_ProjectorWindow.Close();
            }
            this.m_ProjectionImageMatrix.Dispose();
        }
        public override string ToString()
        {
            return this.TitleName;
        }
    }
}
