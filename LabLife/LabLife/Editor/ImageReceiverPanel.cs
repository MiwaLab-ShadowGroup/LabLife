using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace LabLife.Editor
{
    public class ImageReceiverPanel : DefaultPanel
    {
        private int m_ImageReceiveID;
        private Image Image_Main = new Image();
        private TextBlock TextBlock_Header = new TextBlock();
        public int ImageReceiveID
        {
            get
            {
                return this.m_ImageReceiveID;
            }
        }

        public int OpenPortNum
        {
            get
            {
                return ImageReceiverHostPanel.StartPortNum + this.m_ImageReceiveID;
            }
        }


        public ImageReceiverPanel(int id)
        {
            this.m_ImageReceiveID = id; 
            this.TitleName = "Image Receiver" + ImageReceiveID.ToString();
        }
        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            this.TextBlock_Header.Text = this.ToString();
            this.AddContent(this.TextBlock_Header, Dock.Top);
            this.AddContent(this.Image_Main, Dock.Top);
            //Uri test = new Uri(@"C:\workspace\LabLife\LabLife\LabLife\bin\Debug\kakikakefusou.JPG");
            //this.Image_Main.Source = new BitmapImage(test);
        }

        public override string ToString()
        {
            return this.TitleName + "\n" 
                +"\tPort : " + this.OpenPortNum.ToString();
        }
    }
}
