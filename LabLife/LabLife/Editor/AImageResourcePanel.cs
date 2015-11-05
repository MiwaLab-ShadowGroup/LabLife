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
        public delegate void ImageFrameArrivedEventHandler(object Sender, EventArgs e);
        public event ImageFrameArrivedEventHandler ImageFrameArrived;
        protected virtual void OnImageFrameArrived(ImageFrameArrivedEventArgs e)
        {
            if (ImageFrameArrived != null)
            {
                ImageFrameArrived(this, e);
            }
        }
        private int m_DivNum = 0;

        public List<Canvas> Canvas_ImageCallib = new List<Canvas>();
        public UniformGrid Grid_Image = new UniformGrid();

       
        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);
        }
        

        public void SetImageToGridChildren(Image addImage)
        {
            ++m_DivNum;

            this.Grid_Image.Columns = this.m_DivNum;

            this.Grid_Image.Children.Add(addImage);

            var item = new Canvas();

            this.Canvas_ImageCallib.Add(item);

            Grid.SetColumn(item, m_DivNum - 1);
            Grid.SetColumn(addImage, m_DivNum - 1);

        }
    }
}
