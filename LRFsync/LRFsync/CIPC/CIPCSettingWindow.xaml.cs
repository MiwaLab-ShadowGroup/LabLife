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
using System.Windows.Shapes;

namespace CIPCClientWindow
{
    /// <summary>
    /// Interaction logic for CIPCSettingWindow.xaml
    /// </summary>
    public partial class CIPCSettingWindow : Window
    {
        public string ClientName
        {
            set
            {
                this.TextBlock_ClientName.Text = value;
            }
            get
            {
                return this.TextBlock_ClientName.Text;
            }
        }
        public int ClientPort
        {
            set
            {
                this.TextBlock_ClientPort.Text = value.ToString();
            }
            get
            {
                return int.Parse(this.TextBlock_ClientPort.Text);
            }
        }
        public string ServerIPAdress
        {
            set
            {
                this.TextBlock_ServerIPAdress.Text = value;
            }
            get
            {
                return this.TextBlock_ServerIPAdress.Text;
            }
        }
        public int ServerPort
        {
            set
            {
                this.TextBlock_ServerPort.Text = value.ToString();
            }
            get
            {
                return int.Parse(this.TextBlock_ServerPort.Text);
            }
        }
        public CIPC_CS.CLIENT.MODE Mode
        {
            set
            {
                switch (value)
                {
                    case CIPC_CS.CLIENT.MODE.Sender:
                        this.RadioButton_Mode_Sender.IsChecked = true;
                        break;
                    case CIPC_CS.CLIENT.MODE.Receiver:
                        this.RadioButton_Mode_Receiver.IsChecked = true;
                        break;
                    case CIPC_CS.CLIENT.MODE.Both:
                        this.RadioButton_Mode_Both.IsChecked = true;
                        break;
                    case CIPC_CS.CLIENT.MODE.DirectConnect:
                        this.RadioButton_Mode_Receiver.IsChecked = true;
                        break;
                    default:
                        break;
                }
            }
            get
            {
                if (this.RadioButton_Mode_Sender.IsChecked == true)
                {
                    return CIPC_CS.CLIENT.MODE.Sender;
                }
                if (this.RadioButton_Mode_Receiver.IsChecked == true)
                {
                    return CIPC_CS.CLIENT.MODE.Receiver;
                }
                if (this.RadioButton_Mode_Both.IsChecked == true)
                {
                    return CIPC_CS.CLIENT.MODE.Both;
                } 
                if (this.RadioButton_Mode_Direct.IsChecked == true)
                {
                    return CIPC_CS.CLIENT.MODE.DirectConnect;
                }
                return CIPC_CS.CLIENT.MODE.non;
            }
        }
        public int FPS
        {
            set
            {
                this.TextBlock_FPS.Text = value.ToString();
            }
            get
            {
                return int.Parse(this.TextBlock_FPS.Text);
            }
        }

        public CIPCSettingWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (s, e) => this.DragMove();

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
        private void Button_Connect_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

    }
}
