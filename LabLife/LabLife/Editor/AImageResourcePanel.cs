using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace LabLife.Editor
{
    public class ImageFrameArrivedEventArgs : EventArgs
    {
        public ImageFrameArrivedEventArgs(Mat[] mat)
        {
            this.Image = mat;
        }
        public Mat[] Image;
    }

    public abstract class AImageResourcePanel : ADefaultPanel
    {
        public delegate void ImageFrameArrivedEventHandler(object Sender, ImageFrameArrivedEventArgs e);
        public event ImageFrameArrivedEventHandler ImageFrameArrived;
        protected virtual void OnImageFrameArrived(ImageFrameArrivedEventArgs e)
        {
            if (ImageFrameArrived != null)
            {
                ImageFrameArrived(this, e);
            }
        }
        private int m_DivNum = 0;

        public Grid Grid_Image = new Grid();
        public List<ColumnDefinition> ColumnDefinitionList = new List<ColumnDefinition>(); 
        public int ImageNum
        {
            get
            {
                return this.m_DivNum;
            }
        }

        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);
        }

        public void SetImageToGridChildren(Image addImage)
        {
            ++m_DivNum;
            var p = new ColumnDefinition();
            ColumnDefinitionList.Add(p);
            p.Width = new System.Windows.GridLength(1, System.Windows.GridUnitType.Star);
            this.Grid_Image.ColumnDefinitions.Add(p);
            this.Grid_Image.Children.Add(addImage);
            var item = new Canvas();
            Grid.SetColumn(item, m_DivNum - 1);
            Grid.SetColumn(addImage, m_DivNum - 1);
        }

        public void SetGridSize(int index, double width, double height)
        {
            this.ColumnDefinitionList[index].Width = new System.Windows.GridLength(width);
            this.Grid_Image.Width = width * m_DivNum;
            this.Grid_Image.Height = height;

        }
    }
}
