#define UNSTABLE_RECEIVE

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
using System.Windows.Shapes;

namespace LabLife.Processer
{
    public class Transporter : IDisposable
    {
        public enum AdditionMode
        {
            Add,
            Cover,
        }

        private AImageResourcePanel m_srcPanel;
        private ProjectionPanel m_dstPanel;
        private int m_imageIndex;
        private Mat WarpMatrix;
        private List<Thumb> SrcMarkList = new List<Thumb>();
        private List<Thumb> DstMarkList = new List<Thumb>();
        private List<Line> SrcLineList = new List<Line>();
        private List<Line> DstLineList = new List<Line>();
        public Point2f[] src = new Point2f[4];
        public Point2f[] dst = new Point2f[4];
        public Color MainColor;
        private int m_EllipseSize = 30;
        private int m_EllipseThickness = 1;
        private Mat m_LastReceivedImage;

        public AdditionMode m_AdditionMode = AdditionMode.Add;

        private Mat m_Mask;

        public bool IsInitWrap { get; private set; }

        public Canvas Canvas_Src;
        public Canvas Canvas_Dst;

        public Transporter(AImageResourcePanel srcPanel, int ImageIndex, ProjectionPanel dstPanel)
        {
            this.m_srcPanel = srcPanel;
            this.m_dstPanel = dstPanel;
            this.m_imageIndex = ImageIndex;
            this.m_dstPanel.AddSenderList(this);
            this.m_srcPanel.ImageFrameArrived += M_srcPanel_ImageFrameArrived;
            Random r = new Random();
            this.MainColor = Color.FromArgb(255, (byte)r.Next(100, 255), (byte)r.Next(100, 255), (byte)r.Next(100, 255));
            this.AddCanvasSrc(srcPanel.Grid_Image, ImageIndex);
            this.AddCanvasDst(dstPanel.Grid_Image);
            this.IsInitWrap = false;
        }

        /// <summary>
        /// 画像の更新処理
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private void M_srcPanel_ImageFrameArrived(object Sender, ImageFrameArrivedEventArgs e)
        {

            var image = e.Image[this.m_imageIndex];
            if (Sender != this)
            {
                if (this.m_LastReceivedImage != null)
                {
                    this.m_LastReceivedImage.Dispose();
                }
                this.m_LastReceivedImage = image.Clone();

                if (!this.IsInitWrap)
                {
                    this.IsInitWrap = true;
                    this.GetInitWrap(image);
                    this.m_srcPanel.SetGridSize(this.m_imageIndex, image.Width, image.Height);
                }
                this.m_dstPanel.InitImage(this, image);

                var _image = image.WarpPerspective(this.WarpMatrix, this.m_dstPanel.m_ProjectionImageMatrix.Size());


                if (this.m_AdditionMode == AdditionMode.Add)
                {
                    var __image = _image.Clone();
                    __image.SetTo(new Scalar(0, 0, 0));
                    _image = ~_image;
                    _image.CopyTo(__image, m_Mask);
                    this.m_dstPanel.m_ProjectionImageMatrix += __image;
                    __image.Dispose();

                }
                else
                {
                    _image.CopyTo(this.m_dstPanel.m_ProjectionImageMatrix, m_Mask);
                }
                this.m_dstPanel.UpdateImage(this);
                _image.Dispose();
            }
            //thumb操作時
            else
            {
                this.m_dstPanel.InitImageByThumb(image);
                var _image = image.WarpPerspective(this.WarpMatrix, this.m_dstPanel.m_ProjectionImageMatrix.Size());

                if (this.m_AdditionMode == AdditionMode.Add)
                {
                    var __image = _image.Clone();
                    __image.SetTo(new Scalar(0, 0, 0));
                    _image.CopyTo(__image, m_Mask);//残す
                    this.m_dstPanel.m_ProjectionImageMatrix += __image;
                    __image.Dispose();
                }
                else
                {
                    _image.CopyTo(this.m_dstPanel.m_ProjectionImageMatrix, m_Mask);//残す
                }
                this.m_dstPanel.UpdateImageByThumb();
                _image.Dispose();
            }
        }

        private void AddCanvasDst(Grid grid_Image)
        {
            Canvas canvas_image = new Canvas();
            grid_Image.Children.Add(canvas_image);
            this.CreatePointErapseDst(canvas_image);
            this.Canvas_Dst = canvas_image;
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
            Line line1 = new Line();
            line1.Stroke = new SolidColorBrush(this.MainColor);
            line1.StrokeThickness = 1;

            Line line2 = new Line();
            line2.Stroke = new SolidColorBrush(this.MainColor);
            line2.StrokeThickness = 1;

            Line line3 = new Line();
            line3.Stroke = new SolidColorBrush(this.MainColor);
            line3.StrokeThickness = 1;

            Line line4 = new Line();
            line4.Stroke = new SolidColorBrush(this.MainColor);
            line4.StrokeThickness = 1;


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
            this.DstLineList.Add(line1);
            this.DstLineList.Add(line2);
            this.DstLineList.Add(line3);
            this.DstLineList.Add(line4);


            canvas_image.Children.Add(mark1);
            canvas_image.Children.Add(mark2);
            canvas_image.Children.Add(mark3);
            canvas_image.Children.Add(mark4);
            canvas_image.Children.Add(line1);
            canvas_image.Children.Add(line2);
            canvas_image.Children.Add(line3);
            canvas_image.Children.Add(line4);

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
            this.Canvas_Src = canvas_image;
        }

        private void CreatePointErapseSrc(Canvas canvas_image)
        {
            Line line1 = new Line();
            line1.Stroke = new SolidColorBrush(this.MainColor);
            line1.StrokeThickness = 1;

            Line line2 = new Line();
            line2.Stroke = new SolidColorBrush(this.MainColor);
            line2.StrokeThickness = 1;

            Line line3 = new Line();
            line3.Stroke = new SolidColorBrush(this.MainColor);
            line3.StrokeThickness = 1;

            Line line4 = new Line();
            line4.Stroke = new SolidColorBrush(this.MainColor);
            line4.StrokeThickness = 1;

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
            this.SrcLineList.Add(line1);
            this.SrcLineList.Add(line2);
            this.SrcLineList.Add(line3);
            this.SrcLineList.Add(line4);

            canvas_image.Children.Add(mark1);
            canvas_image.Children.Add(mark2);
            canvas_image.Children.Add(mark3);
            canvas_image.Children.Add(mark4);
            canvas_image.Children.Add(line1);
            canvas_image.Children.Add(line2);
            canvas_image.Children.Add(line3);
            canvas_image.Children.Add(line4);

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
#if UNSTABLE_RECEIVE
            if (this.m_LastReceivedImage != null)
            {
                this.M_srcPanel_ImageFrameArrived(this, new ImageFrameArrivedEventArgs(new Mat[] { this.m_LastReceivedImage }));
            }
#endif
        }


        public void SetWarp()
        {
            this.WarpMatrix = Cv2.GetPerspectiveTransform(src, dst);
        }

        private void UpdateLines()
        {

            this.SrcLineList[0].X1 = this.src[0].X;
            this.SrcLineList[0].Y1 = this.src[0].Y;
            this.SrcLineList[0].X2 = this.src[1].X;
            this.SrcLineList[0].Y2 = this.src[1].Y;
            this.SrcLineList[1].X1 = this.src[1].X;
            this.SrcLineList[1].Y1 = this.src[1].Y;
            this.SrcLineList[1].X2 = this.src[2].X;
            this.SrcLineList[1].Y2 = this.src[2].Y;
            this.SrcLineList[2].X1 = this.src[2].X;
            this.SrcLineList[2].Y1 = this.src[2].Y;
            this.SrcLineList[2].X2 = this.src[3].X;
            this.SrcLineList[2].Y2 = this.src[3].Y;
            this.SrcLineList[3].X1 = this.src[3].X;
            this.SrcLineList[3].Y1 = this.src[3].Y;
            this.SrcLineList[3].X2 = this.src[0].X;
            this.SrcLineList[3].Y2 = this.src[0].Y;

            this.DstLineList[0].X1 = this.dst[0].X;
            this.DstLineList[0].Y1 = this.dst[0].Y;
            this.DstLineList[0].X2 = this.dst[1].X;
            this.DstLineList[0].Y2 = this.dst[1].Y;
            this.DstLineList[1].X1 = this.dst[1].X;
            this.DstLineList[1].Y1 = this.dst[1].Y;
            this.DstLineList[1].X2 = this.dst[2].X;
            this.DstLineList[1].Y2 = this.dst[2].Y;
            this.DstLineList[2].X1 = this.dst[2].X;
            this.DstLineList[2].Y1 = this.dst[2].Y;
            this.DstLineList[2].X2 = this.dst[3].X;
            this.DstLineList[2].Y2 = this.dst[3].Y;
            this.DstLineList[3].X1 = this.dst[3].X;
            this.DstLineList[3].Y1 = this.dst[3].Y;
            this.DstLineList[3].X2 = this.dst[0].X;
            this.DstLineList[3].Y2 = this.dst[0].Y;
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
            this.UpdateLines();

            this.m_Mask = new Mat(image.Size(), MatType.CV_8UC1, new Scalar(0));
            List<List<Point>> polygons = new List<List<Point>>();
            List<Point> polygon = new List<Point>();
            polygons.Add(polygon);
            foreach (var p in this.dst)
            {
                polygon.Add(new Point(p.X, p.Y));
            }
            Cv2.FillPoly(this.m_Mask, polygons, new Scalar(255));

            this.SetWarp();
        }
        public void SetupWrap(Point2f[] src, Point2f[] dst)
        {
            this.SetCanvasCenter(this.SrcMarkList[0], src[0].X, src[0].Y);
            this.SetCanvasCenter(this.SrcMarkList[1], src[1].X, src[1].Y);
            this.SetCanvasCenter(this.SrcMarkList[2], src[2].X, src[2].Y);
            this.SetCanvasCenter(this.SrcMarkList[3], src[3].X, src[3].Y);

            this.SetCanvasCenter(this.DstMarkList[0], dst[0].X, dst[0].Y);
            this.SetCanvasCenter(this.DstMarkList[1], dst[1].X, dst[1].Y);
            this.SetCanvasCenter(this.DstMarkList[2], dst[2].X, dst[2].Y);
            this.SetCanvasCenter(this.DstMarkList[3], dst[3].X, dst[3].Y);

            this.m_Mask = new Mat(this.m_LastReceivedImage.Size(), MatType.CV_8UC1, new Scalar(0));
            List<List<Point>> polygons = new List<List<Point>>();
            List<Point> polygon = new List<Point>();
            polygons.Add(polygon);
            foreach (var p in this.dst)
            {
                polygon.Add(new Point(p.X, p.Y));
            }
            Cv2.FillPoly(this.m_Mask, polygons, new Scalar(255));

            this.UpdateLines();
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

            this.m_Mask.Dispose();
            this.m_Mask = new Mat(this.m_LastReceivedImage.Size(), MatType.CV_8UC1, new Scalar(0));
            List<List<Point>> polygons = new List<List<Point>>();
            List<Point> polygon = new List<Point>();
            polygons.Add(polygon);
            foreach (var p in this.dst)
            {
                polygon.Add(new Point(p.X, p.Y));
            }
            Cv2.FillPoly(this.m_Mask, polygons, new Scalar(255));

            this.UpdateLines();
            this.SetWarp();
        }

        public void Dispose()
        {
            this.m_dstPanel.RemoveSenderList(this);
            this.m_srcPanel.ImageFrameArrived -= M_srcPanel_ImageFrameArrived;

            this.RemoveCanvasSrc();
            this.RemoveCanvasDst();
        }

        private void RemoveCanvasDst()
        {
            this.m_dstPanel.Grid_Image.Children.Remove(this.Canvas_Dst);
        }

        private void RemoveCanvasSrc()
        {
            this.m_srcPanel.Grid_Image.Children.Remove(this.Canvas_Src);
        }

        public override string ToString()
        {
            return "{ " + this.m_imageIndex + " of " + m_srcPanel.TitleName + " }  { " + m_dstPanel.TitleName + " }";
        }
    }
}
