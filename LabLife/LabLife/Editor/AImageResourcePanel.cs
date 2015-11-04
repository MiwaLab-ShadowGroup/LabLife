using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
        

        public Canvas Canvas_ImageCallib = new Canvas();
        public Grid Grid_Image = new Grid();

       
        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            this.Grid_Image.Children.Add(Canvas_ImageCallib);
        }
        

        public void SetImageToGridChildren(Image addImage)
        {
            this.Grid_Image.Children.Add(addImage);
        }
    }
}
