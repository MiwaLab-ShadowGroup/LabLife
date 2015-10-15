﻿using System;
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

        public byte[] Recieve()
        {
            Parallel.For(0,this.receiveByte.Length,(i) =>
            {
                this.receiveByte[i].CopyTo(this.receiveRawdata, i * this.receiveByte[0].Length);
            });
            return this.receiveRawdata;
        }

        public void Send(string remoteIPAdress, ushort[] data)
        {
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

            var result = Parallel.For(0, this.ClientNum, (l) =>
            {
                for (int z = 0; z < this.SendByte[l].Length; z += 2)
                {
                    this.SendByte[l][z] = BitConverter.GetBytes(data[z / 2 + l * this.SendByte[0].Length])[0];
                    this.SendByte[l][z] = BitConverter.GetBytes(data[z / 2 + l * this.SendByte[0].Length])[1];
                }
            });
            Parallel.ForEach(this.clients, (p) =>
            {
                p.Value.Send(this.SendByte[p.Key], this.SendByte[p.Key].Length, remoteIPAdress, p.Key + this.StartPortNumber);
            });
        }
    }
}
