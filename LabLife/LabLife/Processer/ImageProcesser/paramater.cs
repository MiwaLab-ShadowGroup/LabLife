using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OpenCvSharp.CPlusPlus;

namespace LabLife.Processer.ImageProcesser
{
    class paramater : LabLife.Editor.AImageResourcePanel
    {
        Slider[] srcSlider;
        Slider[] dstSlider;
        Slider range;

        public paramater()
        {
            base.TitleName = "ColorChangerParam";
            this.srcSlider = new Slider[3];
            this.dstSlider = new Slider[3];
            this.range = new Slider();
            this.range.Maximum = 50;
            this.range.Minimum = 0;
            for(int i = 0; i < 3; i++)
            {
                this.srcSlider[i] = new Slider();
                this.srcSlider[i].Maximum = 255;
                this.srcSlider[i].Minimum = 0;
                this.dstSlider[i] = new Slider();
                this.dstSlider[i].Maximum = 255;
                this.dstSlider[i].Minimum = 0;
            }
         }

        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);
            base.AddContent(this.range, Dock.Top);
            for(int i = 0; i < 3; i++)
            {
                base.AddContent(this.srcSlider[i], Dock.Top);
            }
            for (int i = 0; i < 3; i++)
            {
                base.AddContent(this.dstSlider[i], Dock.Top);
            }
            Border b = new Border();
            b.Style = (Style)App.Current.Resources["Border_Default"];
            base.AddContent(b, Dock.Top);

            this.AddContent(base.Grid_Image, Dock.Top);

            //this.AddContent(canvas1, Dock.Bottom);
        }
    }
}
