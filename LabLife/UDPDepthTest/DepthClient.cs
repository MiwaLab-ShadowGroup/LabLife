using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace UDPDepthTest
{
    class DepthClient
    {
        public byte[][] SendByte { private set; get; }
        public ushort[] Data { private set; get; }
        public byte[][] receiveByte { private set; get; }
        public byte[] receiveRawdata { set; get; }

        public readonly int ClientNum;
        public readonly int StartPortNumber;
        public int rowdatalength;
        public string remoteIPAdress;

        public ushort[] ushortReceivedData { set; get; }

        public List<KeyValuePair<int, UdpClient>> clients { private set; get; }

        /// <summary>
        /// Do not use.
        /// </summary>
        public DepthClient()
            : this(new ushort[160])
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public DepthClient(ushort[] data)
        {
            this.clients = new List<KeyValuePair<int, UdpClient>>();
            this.Data = data;
            this.ClientNum = 10;
            this.StartPortNumber = 35000;
            this.receiveByte = new byte[this.ClientNum][];
            this.receiveRawdata = new byte[512 * 424 * 2];
            this.ushortReceivedData = new ushort[512 * 424];
        }

        /// <summary>
        /// create new instance buffer
        /// </summary>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="remoteIpAdress"></param>
        public DepthClient(int height, int width)
            : this(new ushort[height * width])
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void InitCLients()
        {
            try
            {
                for (int i = 0; i < this.ClientNum; i++)
                {
                    this.clients.Add(new KeyValuePair<int, UdpClient>(i, new UdpClient(this.StartPortNumber + i)));
                    this.clients[i].Value.BeginReceive(this.ReceiveCallbackFunc, this.clients[i]);
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ReceiveCallbackFunc(IAsyncResult ar)
        {
            try
            {
                var u = (KeyValuePair<int, UdpClient>)ar.AsyncState;
                IPEndPoint remoteHost = new IPEndPoint(IPAddress.Any, 0);
                this.receiveByte[u.Key] = u.Value.EndReceive(ar, ref remoteHost);
                //udpcliant.Connect(this.remotehost);
                u.Value.BeginReceive(ReceiveCallbackFunc, u);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var u = (KeyValuePair<int, UdpClient>)ar.AsyncState;
                u.Value.BeginReceive(ReceiveCallbackFunc, u);
            }

        }

        public ushort[] Recieve()
        {
            foreach( var p in this.receiveByte)
            {
                if (p == null)
                {
                    return new ushort[1];
                }
            }
            Parallel.For(0,this.receiveByte.Length,(i) =>
            {
                if (this.receiveByte[i] != null)
                {
                    this.receiveByte[i].CopyTo(this.receiveRawdata, i * this.receiveByte[0].Length);
                }
            });

            for (int i =0; i < this.ushortReceivedData.Length; i++)
            {
                this.ushortReceivedData[i] = BitConverter.ToUInt16( new byte[]{ this.receiveRawdata[i * 2],this.receiveRawdata[i * 2 + 1]},0);
            }

            return this.ushortReceivedData;
        }

        public void Send(string remoteIPAdress, ushort[] data)
        {
            //位置を記録
            this.remoteIPAdress = remoteIPAdress;
            //送信用データの作成
            if (this.rowdatalength != data.Length * 2)
            {
                this.rowdatalength = data.Length * 2;
                this.SendByte = new byte[this.ClientNum][];
                for (int k = 0; k < this.ClientNum - 1; k++)
                {
                    this.SendByte[k] = new byte[data.Length * 2 / this.ClientNum];
                }
                this.SendByte[this.ClientNum - 1] = new byte[data.Length * 2 % this.ClientNum];
            }
            //送信
            var result = Parallel.For(0, this.ClientNum, (l) =>
            {
                for (int z = 0; z < this.SendByte[l].Length; z += 2)
                {
                    this.SendByte[l][z] = BitConverter.GetBytes(data[z / 2 + l * this.SendByte[0].Length/2])[0];
                    this.SendByte[l][z] = BitConverter.GetBytes(data[z / 2 + l * this.SendByte[0].Length/2])[1];
                }
            });
            Parallel.ForEach(this.clients, (p) =>
            {
                p.Value.Send(this.SendByte[p.Key], this.SendByte[p.Key].Length, remoteIPAdress, p.Key + this.StartPortNumber);
            });
        }
    }
}
