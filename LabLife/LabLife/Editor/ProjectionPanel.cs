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

namespace LabLife.Editor
{
    class ProjectionPanel : DefaultPanel
    {
        private int m_ProjectionPanelId;
        private Image Image_Main = new Image();
        private TextBlock TextBlock_Header = new TextBlock();
        private SliderAndTextControl SliderAndTextControl_margin = new SliderAndTextControl();

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
            this.m_ProjectionImageMatrix = new OpenCvSharp.CPlusPlus.Mat(256, 256, OpenCvSharp.CPlusPlus.MatType.CV_8UC4);
            this.m_WritableBitmap = OpenCvSharp.Extensions.WriteableBitmapConverter.ToWriteableBitmap(this.m_ProjectionImageMatrix);
            this.Image_Main.Source = this.m_WritableBitmap;
        }

        private void Slider_Main_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.SliderAndTextControl_margin.Slider_Main.Value = ((int)(this.SliderAndTextControl_margin.Slider_Main.Value));
            this.Image_Main.Margin = new Thickness(this.SliderAndTextControl_margin.Slider_Main.Value);
        }
        
        private void updateImage()
        {

            var p = this.Dispatcher.BeginInvoke(new Action(() =>
            {

            }));

            p.Wait();
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
