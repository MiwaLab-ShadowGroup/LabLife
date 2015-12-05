using LabLife.Contorols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LabLife.Editor
{
    /// <summary>
    /// CIPCを操作して同期用信号の送信を行います
    /// </summary>
    public class SyncRecordManagerCIPCPanel : ADefaultPanel
    {
        public CIPC_CS.CLIENT.CLIENT client;


        public TextAndTextBoxControl TextTextBox_CIPCRemoteIP = new TextAndTextBoxControl();
        public TextAndTextBoxControl TextTextBox_CIPCRemotePort = new TextAndTextBoxControl();
        public TextAndTextBoxControl TextTextBox_CIPCMyPort = new TextAndTextBoxControl();
        public TextAndTextBoxControl TextTextBox_CIPCMyPort_Control = new TextAndTextBoxControl();


        public UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT m_controlClient;


        public SyncRecordManagerCIPCPanel()
        {
            this.TitleName = "SyncRecordManagerCIPCPanel";
        }

        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            Border b1 = new Border();
            StackPanel stp1 = new StackPanel();
            b1.Style = (Style)App.Current.Resources["Border_Default"];
            b1.Child = stp1;
            b1.MinWidth = 150;
            TextBlock tb1 = new TextBlock();
            tb1.Text = "CIPCConnect";
            stp1.Children.Add(tb1);
            this.TextTextBox_CIPCRemoteIP.HeaderText.Text = "RemoteIP";
            this.TextTextBox_CIPCRemoteIP.ContentText.Text = "127.0.0.1";
            stp1.Children.Add(this.TextTextBox_CIPCRemoteIP);
            this.TextTextBox_CIPCRemotePort.HeaderText.Text = "RemotePort";
            this.TextTextBox_CIPCRemotePort.ContentText.Text = "12000";
            stp1.Children.Add(this.TextTextBox_CIPCRemotePort);
            this.TextTextBox_CIPCMyPort.HeaderText.Text = "MyPort";
            this.TextTextBox_CIPCMyPort.ContentText.Text = "15152";
            stp1.Children.Add(this.TextTextBox_CIPCMyPort);

            this.TextTextBox_CIPCMyPort_Control.HeaderText.Text = "MyControlPort";
            this.TextTextBox_CIPCMyPort_Control.ContentText.Text = "15153";
            stp1.Children.Add(this.TextTextBox_CIPCMyPort_Control);

            Button Button_CipcConnect = new Button();
            Button_CipcConnect.Content = "Connect";
            Button_CipcConnect.Click += Button_CipcConnect_Click;
            stp1.Children.Add(Button_CipcConnect);

            Button Button_CipcClose = new Button();
            Button_CipcClose.Content = "Close";
            Button_CipcClose.Click += Button_CipcClose_Click; ;
            stp1.Children.Add(Button_CipcClose);

            Border b2 = new Border();
            StackPanel stp2 = new StackPanel();
            b2.Style = (Style)App.Current.Resources["Border_Default"];
            b2.Child = stp2;
            TextBlock tb2 = new TextBlock();
            tb2.Text = "Control";
            Button Button_CIPCSenderSTART = new Button();
            Button_CIPCSenderSTART.Content = "START";
            Button_CIPCSenderSTART.Click += Button_CIPCSender_Click;

            Button Button_CIPCSenderSTOP = new Button();
            Button_CIPCSenderSTOP.Content = "STOP";
            Button_CIPCSenderSTOP.Click += Button_CIPCSenderSTOP_Click; ;

            stp2.Children.Add(tb2);
            stp2.Children.Add(Button_CIPCSenderSTART);
            stp2.Children.Add(Button_CIPCSenderSTOP);


            base.AddContent(b1, Dock.Left);
            base.AddContent(b2, Dock.Left);
        }

        private void Button_CipcClose_Click(object sender, RoutedEventArgs e)
        {
            this.client.Close();
        }

        private void Button_CIPCSenderSTOP_Click(object sender, RoutedEventArgs e)
        {
            Udp_Send( new TerminalConnectionSettings.TerminalProtocols.SaveConnectionFast());
            Thread.Sleep(100);
            byte[] data = null;
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += "STOP";
            data = enc.data;
            this.client.Update(ref data);

            Thread.Sleep(100);

            Udp_Send(new TerminalConnectionSettings.TerminalProtocols.AllDisConnect());

            Udp_Send(new TerminalConnectionSettings.TerminalProtocols.TurnOnSyncConnect());
            Udp_Send(new TerminalConnectionSettings.TerminalProtocols.LoadConnectionFast());
        }

        private void Udp_Send(TerminalConnectionSettings.TerminalProtocols.CIPCTerminalCommand terminalcommand)
        {
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += (int)20;
            enc += (string)terminalcommand.Data;
            this.m_controlClient.Send(enc.data);
        }

        private void Button_CIPCSender_Click(object sender, RoutedEventArgs e)
        {
            byte[] data = null;
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += "START";
            data = enc.data;
            this.client.Update(ref data);
        }

        private void Button_CipcConnect_Click(object sender, RoutedEventArgs e)
        {
            this.client = new CIPC_CS.CLIENT.CLIENT(int.Parse(this.TextTextBox_CIPCMyPort.ContentText.Text),
                this.TextTextBox_CIPCRemoteIP.ContentText.Text,
                int.Parse(this.TextTextBox_CIPCRemotePort.ContentText.Text), "SyncManager", 30);

            this.client.Setup(CIPC_CS.CLIENT.MODE.Sender);

            if (this.m_controlClient == null)
            {
                this.m_controlClient = new UDP_PACKETS_CLIANT.UDP_PACKETS_CLIANT(this.TextTextBox_CIPCRemoteIP.ContentText.Text,
                    int.Parse(this.TextTextBox_CIPCRemotePort.ContentText.Text),
                    int.Parse(this.TextTextBox_CIPCMyPort_Control.ContentText.Text));
            }
        }

        public override void Close(object sender, RoutedEventArgs e)
        {
            base.Close(sender, e);
        }

    }
}
