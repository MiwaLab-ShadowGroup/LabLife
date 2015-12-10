using System;
using System.Collections.Generic;
using System.Linq;
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

namespace CIPCClientWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CIPC_CS.CLIENT.CLIENT client { set; get; }

        public delegate void DataReceivedEventHandler(object sender, byte[] e);
        public event DataReceivedEventHandler DataReceived;
        protected virtual void OnDataReceived(byte[] e)
        {
            if (this.DataReceived != null) { this.DataReceived(this, e); }
        }

        public string TitleName
        {
            set
            {
                this.TextBlock_TitleName.Text = value;
            }
            get
            {
                return this.TextBlock_TitleName.Text;
            }
        }

        public MainWindow(int myport, string serverIP, int serverport, string clientname, CIPC_CS.CLIENT.MODE mode,int FPS, string title = "CIPCClient")
        {
            try
            {
                InitializeComponent();
                this.TextBlock_TitleName.Text = title;
                this.TextBlock_ClientName.Text = clientname;
                this.TextBlock_ClientPort.Text = myport.ToString();
                this.TextBlock_ServerIPAdress.Text = serverIP;
                this.TextBlock_ServerPort.Text = serverport.ToString();
                this.TextBlock_Mode.Text = mode.ToString();
                this.TextBlock_FPS.Text = FPS.ToString();



                this.MouseLeftButtonDown += (s, e) => this.DragMove();
                this.TitleName = "CIPCClient";
                this.client = new CIPC_CS.CLIENT.CLIENT(myport, serverIP, serverport, clientname, FPS);
                this.client.Setup(mode);
                this.client.DataReceived += client_DataReceived;
                this.Closing += MainWindow_Closing;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        public MainWindow(string title, CIPCClientWindow.CIPCSettingWindow settingwindow)
        {
            try
            {
                InitializeComponent();
                this.TextBlock_TitleName.Text = title;
                this.TextBlock_ClientName.Text = settingwindow.ClientName;
                this.TextBlock_ClientPort.Text = settingwindow.ClientPort.ToString();
                this.TextBlock_ServerIPAdress.Text = settingwindow.ServerIPAdress;
                this.TextBlock_ServerPort.Text = settingwindow.ServerPort.ToString();
                this.TextBlock_Mode.Text = settingwindow.Mode.ToString();
                this.TextBlock_FPS.Text = settingwindow.FPS.ToString();



                this.MouseLeftButtonDown += (s, e) => this.DragMove();
                this.TitleName = "CIPCClient";
                this.client = new CIPC_CS.CLIENT.CLIENT(settingwindow.ClientPort, settingwindow.ServerIPAdress, settingwindow.ServerPort, settingwindow.ClientName, settingwindow.FPS);
                this.client.Setup(settingwindow.Mode);
                this.client.DataReceived += client_DataReceived;
                this.Closing += MainWindow_Closing;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            try
            {
                this.Border_Focus.BorderBrush = Brushes.Blue;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void client_DataReceived(object sender, byte[] e)
        {
            this.OnDataReceived(e);
        }

        public void send(byte[] data)
        {
            this.client.Update(ref data);
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            try
            {
                this.Border_Focus.BorderBrush = Brushes.Brown;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
