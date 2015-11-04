using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LabLife.Editor
{
    public abstract class AImageResourcePanel : ADefaultPanel
    {
        private Mat m_ImageResource;

        public Canvas Canvas_ImageCallib = new Canvas();
        public Grid Grid_Image = new Grid();

        public Mat ImageResource
        {
            get
            {
                return this.m_ImageResource;
            }
        }

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
