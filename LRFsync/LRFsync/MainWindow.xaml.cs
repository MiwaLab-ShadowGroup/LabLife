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
using System.Threading;

namespace LRFsync
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        List<LRFInfomation> List_LRF;

        CIPC_CS.CLIENT.CLIENT client;
        Timer timer;

        public MainWindow()
        {
            InitializeComponent();

            

            List_LRF = new List<LRFInfomation>();
        }


        private void Add_Client_Click(object sender, RoutedEventArgs e)
        {
            int myPort = int.Parse(this.TextBlock_ClientPort.Text);
            string serverIP = this.TextBlock_ServerIPAdress.Text;
            int serverPort = int.Parse(this.TextBlock_ServerPort.Text);
            string clientName = this.TextBlock_ClientName.Text;
            int fps = int.Parse(this.TextBlock_FPS.Text);
            int id = int.Parse(this.TextBox_Id.Text);

            try
            {
                //新しくアカウントを立てる
                CIPCClient.ReceiveClient newClient = this.Create_CIPCClient(myPort, serverIP, serverPort, clientName, fps, id);
                //KinectList追加
                this.ADD_LRF(id, newClient);
            }
            catch
            {
                Console.WriteLine("Add_Client_Click");
            }


            //次のkinectの準備
            myPort++;
            id++;
            this.TextBox_Id.Text = id.ToString();
            this.TextBlock_ClientName.Text = "CIPCClient" + id.ToString();
            this.TextBlock_ClientPort.Text = myPort.ToString();
        }


        void ADD_LRF(int id, CIPCClient.ReceiveClient client)
        {

            try
            {
                LRFInfomation newLRF = new LRFInfomation();
                newLRF.Id = id;
                newLRF.client = client;
                this.List_LRF.Add(newLRF);
            }
            catch
            {
                Console.WriteLine("ADD_Kinect");
            }

        }

        CIPCClient.ReceiveClient Create_CIPCClient(int myPort, string serverIP, int serverPort, string clientName, int fps, int id)
        {
            CIPCClient.ReceiveClient newClient = new CIPCClient.ReceiveClient(myPort, serverIP, serverPort, clientName, fps, id);
            try
            {
                newClient.Setup(CIPC_CS.CLIENT.MODE.Both);
                newClient.DataReceived += this.DataReceived;

            }
            catch
            {
                Console.WriteLine("Create_CIPCClient");
            }
            return newClient;
        }

        void DataReceived(object sender, byte[] e)
        {
            try
            {

                
                CIPCClient.ReceiveClient senderclient = (CIPCClient.ReceiveClient)sender;
                int id = senderclient.ID;

                List<System.Windows.Point> list_human = new List<Point>();
                UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                dec.Source = e;

                int humanNum = dec.get_int();
                for (int i = 0; i < humanNum; i++)
                {
                    float x = (float)dec.get_double();
                    float z = (float)dec.get_double();
                    list_human.Add(new Point(x,z));
                }

                this.List_LRF[id].listLRFdata = list_human;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        void Add_SendClient_Click(object sender, RoutedEventArgs e)
        {

            int mysendport = int.Parse(this.TextBlock_SendClientPort.Text);//54000;
            string sendserverIP = this.TextBlock_SendServerIPAdress.Text;
            int sendserverPort = int.Parse(this.TextBlock_SendServerPort.Text);//50000;
            string sendclientName = "SenderClient_For_Media";
            int sendfps = 30;

            CIPC_CS.CLIENT.CLIENT client = this.CreateSenderClient(mysendport, sendserverIP, sendserverPort, sendclientName, sendfps);
            mysendport++;
            this.TextBlock_SendClientPort.Text = mysendport.ToString();
          

        }
        CIPC_CS.CLIENT.CLIENT CreateSenderClient(int mysendport, string sendserverIP, int sendserverPort, string sendclientName, int sendfps)
        {
             this.client = new CIPC_CS.CLIENT.CLIENT(mysendport, sendserverIP, sendserverPort, sendclientName, sendfps);

            //List<LRFInfomation> list_kinectInfo = new List<LRFInfomation>();

            try
            {
                client.Setup(CIPC_CS.CLIENT.MODE.Both);
                this.timer = new System.Threading.Timer(new System.Threading.TimerCallback(this.senddata), client, 0, 33);


                // Console.WriteLine("OK");
            }
            catch
            {
                Console.WriteLine("createsender");
            }
            return client;
        }

        public void senddata(object state)
        {

            try
            {
                List<Point> list = new List<Point>();
                for(int i= 0; i < this.List_LRF.Count; i++)
                {
                    for (int j = 0; j < this.List_LRF[i].listLRFdata.Count; j++)
                    {
                        list.Add(this.List_LRF[i].listLRFdata[j]);
                    }

                   
                }
                UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
                enc += list.Count;
                for (int i = 0; i < list.Count; i++)
                {
                    enc += list[i].X;
                    enc += list[i].Y;
                }
                byte[] data = enc.data;
                client.Update(ref data);

                Console.WriteLine("send");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

    }
}
