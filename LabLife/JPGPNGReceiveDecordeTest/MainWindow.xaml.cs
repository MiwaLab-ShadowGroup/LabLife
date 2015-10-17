using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JPGPNGReceiveDecordeTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapSource bitmapsorce;
        private JpegBitmapDecoder jpegDec;
        private System.IO.MemoryStream receivedData;
        private UdpClient udpclient;
        private IPEndPoint remoteEP;

        public MainWindow()
        {
            InitializeComponent();
            udpclient = new System.Net.Sockets.UdpClient(15000);
            udpclient.BeginReceive(receiveCallback, udpclient);

            this.receivedData = new System.IO.MemoryStream();

        }

        private void receiveCallback(IAsyncResult ar)
        {
            try
            {
                this.receivedData.Close();
                this.receivedData = new System.IO.MemoryStream( this.udpclient.EndReceive(ar, ref this.remoteEP));
                this.updateImage();
                Console.WriteLine(this.receivedData.Length.ToString());
                udpclient.BeginReceive(receiveCallback, udpclient);
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);

            }
        }

        private void updateImage()
        {
            
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.jpegDec = new JpegBitmapDecoder(this.receivedData, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                this.bitmapsorce = this.jpegDec.Frames[0];
                this.TestImage.Source = this.bitmapsorce;
            }));
        }
    }
}
