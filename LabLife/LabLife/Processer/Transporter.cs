using LabLife.Data;
using LabLife.Editor;
using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using LabLife.Contorols;
using System.Windows.Media;

namespace LabLife.Processer
{
    public class Transporter : IDisposable
    {
        private AImageResourcePanel m_srcPanel;
        private ProjectionPanel m_dstPanel;
        private int m_imageIndex;
        private Mat WarpMatrix;
        private List<Thumb> SrcMarkList = new List<Thumb>();
        private List<Thumb> DstMarkList = new List<Thumb>();
        private Point2f[] src = new Point2f[4];
        private Point2f[] dst = new Point2f[4];
        private Color MainColor;
        private int m_EllipseSize = 30;
        private int m_EllipseThickness = 1;

        public bool IsInitWrap { get; private set; }

        public Transporter(AImageResourcePanel srcPanel, int ImageIndex, ProjectionPanel dstPanel)
        {
            this.m_srcPanel = srcPanel;
            this.m_dstPanel = dstPanel;
            this.m_imageIndex = ImageIndex;
            this.m_dstPanel.AddSenderList(this);
            this.m_srcPanel.ImageFrameArrived += M_srcPanel_ImageFrameArrived;
            Random r = new Random();
            this.MainColor = Color.FromArgb(255, (byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255));
            this.AddCanvasSrc(srcPanel.Grid_Image, ImageIndex);
            this.AddCanvasDst(dstPanel.Grid_Image);
            this.IsInitWrap = false;
        }
        private void AddCanvasDst(Grid grid_Image)
        {
            Canvas canvas_image = new Canvas();
            grid_Image.Children.Add(canvas_image);
            this.CreatePointErapseDst(canvas_image);
        }
        private void SetCanvasCenter(Thumb item, double x, double y)
        {
            Canvas.SetLeft(item, x - item.Width / 2);
            Canvas.SetTop(item, y - item.Height / 2);
        }
        private Point2f GetCanvasCenter(Thumb item)
        {
            var x = Canvas.GetLeft(item) + item.Width / 2;
            var y = Canvas.GetTop(item) + item.Height / 2;
            return new Point2f((float)x, (float)y);
        }
        private void CreatePointErapseDst(Canvas canvas_image)
        {
            Thumb mark1 = new Thumb();
            mark1.Template = (ControlTemplate)App.Current.Resources["ThumbEllipseTemplate"];
            mark1.Background = new SolidColorBrush(Colors.Transparent);
            mark1.BorderBrush = new SolidColorBrush(this.MainColor);
            mark1.ApplyTemplate();
            mark1.Width = m_EllipseSize;
            mark1.Height = m_EllipseSize;
            mark1.DragDelta += mark_DragDelta;

            Thumb mark2 = new Thumb();
            mark2.Template = (ControlTemplate)App.Current.Resources["ThumbEllipseTemplate"];
            mark2.Background = new SolidColorBrush(Colors.Transparent);
            mark2.BorderBrush = new SolidColorBrush(this.MainColor);
            mark2.Width = m_EllipseSize;
            mark2.Height = m_EllipseSize;
            mark2.DragDelta += mark_DragDelta;

            Thumb mark3 = new Thumb();
            mark3.Template = (ControlTemplate)App.Current.Resources["ThumbEllipseTemplate"];
            mark3.Background = new SolidColorBrush(Colors.Transparent);
            mark3.BorderBrush = new SolidColorBrush(this.MainColor);
            mark3.Width = m_EllipseSize;
            mark3.Height = m_EllipseSize;
            mark3.DragDelta += mark_DragDelta;

            Thumb mark4 = new Thumb();
            mark4.Template = (ControlTemplate)App.Current.Resources["ThumbEllipseTemplate"];
            mark4.Background = new SolidColorBrush(Colors.Transparent);
            mark4.BorderBrush = new SolidColorBrush(this.MainColor);
            mark4.Width = m_EllipseSize;
            mark4.Height = m_EllipseSize;
            mark4.DragDelta += mark_DragDelta;

            this.DstMarkList.Add(mark1);
            this.DstMarkList.Add(mark2);
            this.DstMarkList.Add(mark3);
            this.DstMarkList.Add(mark4);

            canvas_image.Children.Add(mark1);
            canvas_image.Children.Add(mark2);
            canvas_image.Children.Add(mark3);
            canvas_image.Children.Add(mark4);

            Canvas.SetLeft(mark1, 0);
            Canvas.SetTop(mark1, 0);
            Canvas.SetLeft(mark2, 0);
            Canvas.SetTop(mark2, 0);
            Canvas.SetLeft(mark3, 0);
            Canvas.SetTop(mark3, 0);
            Canvas.SetLeft(mark4, 0);
            Canvas.SetTop(mark4, 0);
        }

        private void AddCanvasSrc(Grid grid_Image, int imageIndex)
        {
            Canvas canvas_image = new Canvas();
            grid_Image.Children.Add(canvas_image);
            Grid.SetColumn(canvas_image, imageIndex);
            this.CreatePointErapseSrc(canvas_image);
        }

        private void CreatePointErapseSrc(Canvas canvas_image)
        {
            Thumb mark1 = new Thumb();
            mark1.Template = (ControlTemplate)App.Current.Resources["ThumbEllipseTemplate"];
            mark1.Background = new SolidColorBrush(Colors.Transparent);
            mark1.BorderBrush = new SolidColorBrush(this.MainColor);
            mark1.Width = m_EllipseSize;
            mark1.Height = m_EllipseSize;
            mark1.DragDelta += mark_DragDelta;

            Thumb mark2 = new Thumb();
            mark2.Template = (ControlTemplate)App.Current.Resources["ThumbEllipseTemplate"];
            mark2.Background = new SolidColorBrush(Colors.Transparent);
            mark2.BorderBrush = new SolidColorBrush(this.MainColor);
            mark2.Width = m_EllipseSize;
            mark2.Height = m_EllipseSize;
            mark2.DragDelta += mark_DragDelta;

            Thumb mark3 = new Thumb();
            mark3.Template = (ControlTemplate)App.Current.Resources["ThumbEllipseTemplate"];
            mark3.Background = new SolidColorBrush(Colors.Transparent);
            mark3.BorderBrush = new SolidColorBrush(this.MainColor);
            mark3.Width = m_EllipseSize;
            mark3.Height = m_EllipseSize;
            mark3.DragDelta += mark_DragDelta;

            Thumb mark4 = new Thumb();
            mark4.Template = (ControlTemplate)App.Current.Resources["ThumbEllipseTemplate"];
            mark4.Background = new SolidColorBrush(Colors.Transparent);
            mark4.BorderBrush = new SolidColorBrush(this.MainColor);
            mark4.Width = m_EllipseSize;
            mark4.Height = m_EllipseSize;
            mark4.DragDelta += mark_DragDelta;

            this.SrcMarkList.Add(mark1);
            this.SrcMarkList.Add(mark2);
            this.SrcMarkList.Add(mark3);
            this.SrcMarkList.Add(mark4);

            canvas_image.Children.Add(mark1);
            canvas_image.Children.Add(mark2);
            canvas_image.Children.Add(mark3);
            canvas_image.Children.Add(mark4);
            Canvas.SetLeft(mark1, 0);
            Canvas.SetTop(mark1, 0);
            Canvas.SetLeft(mark2, 0);
            Canvas.SetTop(mark2, 0);
            Canvas.SetLeft(mark3, 0);
            Canvas.SetTop(mark3, 0);
            Canvas.SetLeft(mark4, 0);
            Canvas.SetTop(mark4, 0);
        }

        /// <summary>
        /// ドラッグ中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mark_DragDelta(object sender,
            System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var mark = sender as Thumb;
            Canvas.SetLeft(mark, Canvas.GetLeft(mark) + e.HorizontalChange);
            Canvas.SetTop(mark, Canvas.GetTop(mark) + e.VerticalChange);
            this.UpdateWarp();
        }


        public void SetWarp()
        {
            this.WarpMatrix = Cv2.GetPerspectiveTransform(src, dst);
        }


        private void M_srcPanel_ImageFrameArrived(object Sender, ImageFrameArrivedEventArgs e)
        {

            var image = e.Image[this.m_imageIndex];
            if (!this.IsInitWrap)
            {
                this.IsInitWrap = true;
                this.GetInitWrap(image);
                this.m_srcPanel.SetGridSize(this.m_imageIndex, image.Width, image.Height);
            }
            this.m_dstPanel.InitImage(this, image);

            var _image = image.WarpPerspective(this.WarpMatrix, this.m_dstPanel.m_ProjectionImageMatrix.Size());

            this.m_dstPanel.m_ProjectionImageMatrix += _image;
            this.m_dstPanel.UpdateImage(this);
            _image.Dispose();
        }

        private void GetInitWrap(Mat image)
        {
            this.src[0] = new Point2f(0, 0);
            this.src[1] = new Point2f(0, image.Height);
            this.src[2] = new Point2f(image.Width, image.Height);
            this.src[3] = new Point2f(image.Width, 0);

            this.dst[0] = new Point2f(0, 0);
            this.dst[1] = new Point2f(0, image.Height);
            this.dst[2] = new Point2f(image.Width, image.Height);
            this.dst[3] = new Point2f(image.Width, 0);

            this.SetCanvasCenter(this.SrcMarkList[0], 0, 0);
            this.SetCanvasCenter(this.SrcMarkList[1], 0, image.Height);
            this.SetCanvasCenter(this.SrcMarkList[2], image.Width, image.Height);
            this.SetCanvasCenter(this.SrcMarkList[3], image.Width, 0);

            this.SetCanvasCenter(this.DstMarkList[0], 0, 0);
            this.SetCanvasCenter(this.DstMarkList[1], 0, image.Height);
            this.SetCanvasCenter(this.DstMarkList[2], image.Width, image.Height);
            this.SetCanvasCenter(this.DstMarkList[3], image.Width, 0);

            this.SetWarp();
        }

        private void UpdateWarp()
        {
            this.src[0] = GetCanvasCenter(this.SrcMarkList[0]);
            this.src[1] = GetCanvasCenter(this.SrcMarkList[1]);
            this.src[2] = GetCanvasCenter(this.SrcMarkList[2]);
            this.src[3] = GetCanvasCenter(this.SrcMarkList[3]);

            this.dst[0] = GetCanvasCenter(this.DstMarkList[0]);
            this.dst[1] = GetCanvasCenter(this.DstMarkList[1]);
            this.dst[2] = GetCanvasCenter(this.DstMarkList[2]);
            this.dst[3] = GetCanvasCenter(this.DstMarkList[3]);

            this.SetWarp();
        }

        public void Dispose()
        {
            this.m_dstPanel.RemoveSenderList(this);
            this.m_srcPanel.ImageFrameArrived -= M_srcPanel_ImageFrameArrived;
        }

        public override string ToString()
        {
            return "{ " + this.m_imageIndex + " of " + m_srcPanel.TitleName + " }  { " + m_dstPanel.TitleName + " }";
        }
    }
}
