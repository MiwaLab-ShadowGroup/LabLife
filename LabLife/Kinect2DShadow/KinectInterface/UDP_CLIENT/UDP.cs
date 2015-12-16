using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDP
{
    public delegate void _DataReceived(object sender, byte[] e);

    /// <summary>
    /// サーバー側でポート2100を開放
    /// クライアント側で2000を開放
    /// 2000→2100にアクセスして，サーバー側にローカルクライアントをててる
    /// 新しいポートでローカルクライアントと接続
    /// </summary>
    class UDP_CLIENT_AUT 
    {
        #region
        public int myPort{get; protected set;}
        string myIP;
        public int serverPort{ get; protected set; }
        public string serverIP { get; protected set; }
        
        System.Net.Sockets.UdpClient client;

        bool IsAccessed;
        public event _DataReceived DataReceived;
        public delegate void _MakeNewClient(int newmyport, int newclientport);
        public event _MakeNewClient makenewclient;
        #endregion
        

        //コンストラクタ
        public UDP_CLIENT_AUT()
        {
            this.IsAccessed = false;
            this.myPort = 2000;
            this.serverPort = 2100;
            this.serverIP = "127.0.0.1";
            this.myIP = "127.0.0.1";
            this.setup();
        }
        public UDP_CLIENT_AUT(string remoteIP)
        {
            this.IsAccessed = false;
            this.myPort = 2000;
            this.serverPort = 2100;
            this.serverIP = remoteIP;
            this.myIP = "127.0.0.1";
            this.setup();
        }

        void setup()
        {
            //接続
            System.Net.IPEndPoint localEP = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(this.myIP), this.myPort);
            this.client = new System.Net.Sockets.UdpClient(localEP);
            this.client.BeginReceive(this.AccessReceiveCallback, this.client);
            Console.WriteLine("clientaut");

        }
        //アクセス要求
        void AccessReceiveCallback(IAsyncResult ar)
        {
            Console.WriteLine("rec:c");
            //受信中止
            System.Net.Sockets.UdpClient client = (System.Net.Sockets.UdpClient)ar.AsyncState;
            System.Net.IPEndPoint remoteEP = null;
            byte[] recieveData;
            try
            {
                recieveData = client.EndReceive(ar, ref remoteEP);
                UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                dec.Source = recieveData;
                bool state = dec.get_bool();
                if (state)
                {
                    //サーバーが返したポートに接続しなおし
                    this.MakeNewClient(dec.get_int(), dec.get_int());
                    this.IsAccessed = true;
                }

                
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return;
            }
            //受信再開
            if (!this.IsAccessed)
            {
                client.BeginReceive(this.AccessReceiveCallback, client);
            }
        }
        public void Access()
        {
            try
            {
                UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
                enc += true;
                byte[] data = enc.data;
                this.client.BeginSend(data, data.Length, this.serverIP, this.serverPort, this.AccesSsendCallBack, this.client);
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        void AccesSsendCallBack(IAsyncResult ar)
        {
            System.Net.Sockets.UdpClient client = (System.Net.Sockets.UdpClient)ar.AsyncState;
            //送信終了
            try
            {
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:callback");
            }

        }
        void MakeNewClient(int newServerPort, int newMyPort)
        {
            try
            {
                this.serverPort = newServerPort;
                this.myPort = newMyPort;
                //再バインド
                System.Net.IPEndPoint localEP = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(this.myIP), this.myPort);
                this.client.Close();
                this.client = new System.Net.Sockets.UdpClient(localEP);
                //受信開始
                this.client.BeginReceive(this.ReceiveCallBack, this.client);
                this.makenewclient(newMyPort, newServerPort);
                Console.WriteLine("newClinet");
            }
            catch
            {
                Console.WriteLine("Error:createClient;");
            }
            
        }

        //接続してからの処理
        void ReceiveCallBack(IAsyncResult ar)
        {
            //受信中止
            System.Net.Sockets.UdpClient localClient = (System.Net.Sockets.UdpClient)ar.AsyncState;
            System.Net.IPEndPoint remoteEP = null;
            byte[] recieveData;
            try
            {
                recieveData = localClient.EndReceive(ar, ref remoteEP);
                //受信した時の処理
                this.DataReceived(ar, recieveData);

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return;
            }

            //受信再開
            localClient.BeginReceive(this.ReceiveCallBack, localClient);

        }
        public void Send(byte[] data)
        {
            //クライアントがなければ作る
            if (this.client != null)
                this.client.BeginSend(data, data.Length, this.serverIP, this.serverPort, this.sendCallBack, this.client);

        }
        void sendCallBack(IAsyncResult ar)
        {
            //送信終了
            System.Net.Sockets.UdpClient client = (System.Net.Sockets.UdpClient)ar.AsyncState;
            try
            {
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

        }

        public void Close()
        {
            this.client.Close();
        }
    }

    class UDP_SERVER
    {
        #region
        //アクセス待ち
        int myPort;
        int clientPort;
        string myIP;
        string clientIP;
        System.Net.Sockets.UdpClient server;
        //ローカルに割り当てる
        public int localPort { get; protected set; }
        public int newclientPort { get; protected set; }
        public List<UDP_CLIENT> localClients{get; protected set;}
        public delegate void _MakeLocalClient(int clientport, int licalport, string clientIP);
        public event _MakeLocalClient makelocalclient;
        #endregion

        public UDP_SERVER()
        {
            this.myPort = 2100;
            this.clientPort = 2000;
            this.myIP = "127.0.0.1";
            this.clientIP = "127.0.0.1";
            this.localPort = 2101;
            this.newclientPort = 2001;
            this.localClients = new List<UDP_CLIENT>();
            this.MakeUDPServer();

        }
        void MakeUDPServer()
        {
            try
            {
                //接続
                System.Net.IPEndPoint localEP = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(this.myIP), this.myPort);
                this.server = new System.Net.Sockets.UdpClient(localEP);
                this.server.BeginReceive(ReceiveCallback, this.server);
                Console.WriteLine("server");
            }
            catch
            {
                Console.WriteLine("Error:createServer");
            }
            
        }
        void ReceiveCallback(IAsyncResult ar)
        {
            Console.WriteLine("rec:s");
            //受信中止
            System.Net.Sockets.UdpClient localClient = (System.Net.Sockets.UdpClient)ar.AsyncState;
            System.Net.IPEndPoint remoteEP = null;
            byte[] recieveData;
            try
            {
                recieveData = localClient.EndReceive(ar, ref remoteEP);
                UDP_PACKETS_CODER.UDP_PACKETS_DECODER dec = new UDP_PACKETS_CODER.UDP_PACKETS_DECODER();
                dec.Source = recieveData;
                if (dec.get_bool()) 
                {
                    //アクセス要求が来たら返信
                    this.MakeNewLocalClient(remoteEP);
                }       
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return;
            }
            //受信再開
            localClient.BeginReceive(ReceiveCallback, localClient);
        }

        void MakeNewLocalClient(System.Net.IPEndPoint EP)
        {
            try
            {
                UDP_CLIENT newclient = new UDP_CLIENT(EP.Address.ToString(), this.newclientPort, this.localPort);
                //接続
                this.localClients.Add(newclient);
                //受信開始
                newclient.StartReceive();
                this.makelocalclient(this.newclientPort, this.localPort, EP.Address.ToString());
                this.Send(this.localPort, this.newclientPort);
                this.localPort++;
                this.newclientPort++;
                Console.WriteLine("client");
            }
            catch
            {
                Console.WriteLine("Error:makenewclient");
            }
            
        }

        void Send(int localport, int newclientport)
        {
            UDP_PACKETS_CODER.UDP_PACKETS_ENCODER enc = new UDP_PACKETS_CODER.UDP_PACKETS_ENCODER();
            enc += true;
            enc += localport;
            enc += newclientport;
            byte[] data = enc.data;

            if (this.server != null)
                this.server.BeginSend(data, data.Length, this.clientIP, this.clientPort, this.sendCallBack, this.server);

        }
        void sendCallBack(IAsyncResult ar)
        {
            //送信終了
            System.Net.Sockets.UdpClient server = (System.Net.Sockets.UdpClient)ar.AsyncState;
            try
            {
                server.EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

        }

        public void Close()
        {
            this.server.Close();
            for (int i = 0; i < this.localClients.Count; i++)
            {
                this.localClients[i].Close();
            }
        }
    }

    class UDP_CLIENT
    {
        #region
        public int myPort{ get; protected set; }
        string myIP;
        public int serverPort { get; protected set; }
        public string serverIP { get; protected set; }

        System.Net.Sockets.UdpClient client;

        public event _DataReceived DataReceived;
        #endregion


        //コンストラクタ
        public UDP_CLIENT()
        {
            this.myPort = 2000;
            this.serverPort = 2100;
            this.serverIP = "127.0.0.1";
            this.myIP = "127.0.0.1";
            this.setup();
        }
        public UDP_CLIENT(string remoteIP, int remotePort, int myPort)
        {
            
            this.myPort = myPort;
            this.serverPort = remotePort;
            this.serverIP = remoteIP;
            this.myIP = "127.0.0.1";
            this.setup();
        }
        void setup()
        {
            //接続
            System.Net.IPEndPoint localEP = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(this.myIP), this.myPort);
            this.client = new System.Net.Sockets.UdpClient(localEP);
            this.client.BeginReceive(this.ReceiveCallBack, this.client);
        }

        public void StartReceive()
        {
            this.client.BeginReceive(this.ReceiveCallBack, this.client);
        }

        //接続してからの処理
        void ReceiveCallBack(IAsyncResult ar)
        {
            //受信中止
            System.Net.Sockets.UdpClient localClient = (System.Net.Sockets.UdpClient)ar.AsyncState;
            System.Net.IPEndPoint remoteEP = null;
            byte[] recieveData;
            try
            {
                recieveData = localClient.EndReceive(ar, ref remoteEP);
                //受信した時の処理
                this.DataReceived(ar, recieveData);

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return;
            }

            //受信再開
            localClient.BeginReceive(this.ReceiveCallBack, localClient);

        }
        public void Send(byte[] data)
        {
            //クライアントがなければ作る
            try
            {
                if (this.client == null)
                    this.client = new System.Net.Sockets.UdpClient();
                this.client.BeginSend(data, data.Length, this.serverIP, this.serverPort, this.sendCallBack, this.client);
            }
            catch
            {

            }

        }
        void sendCallBack(IAsyncResult ar)
        {
            //送信終了
            System.Net.Sockets.UdpClient client = (System.Net.Sockets.UdpClient)ar.AsyncState;
            try
            {
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

        }

        public void Close()
        {
            this.client.Close();
        }
    }
}
