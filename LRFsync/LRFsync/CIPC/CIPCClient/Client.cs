using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRFsync.CIPCClient
{
    public class ReceiveClient
    {
        public CIPC_CS.CLIENT.CLIENT client;
        public string ServerIPAdress { get; private set; }
        public int ServerPortNumber { get; private set; }
        public int MyPortNumber { get; private set; }
        public string Name { get; private set; }
        public int fps { get; private set; }
        public int ID { get; private set; }

        public ReceiveClient(int myport, string serverip, int serverport, string name, int fps, int id)
        {
            this.ServerIPAdress = serverip;
            this.ServerPortNumber = serverport;
            this.MyPortNumber  = myport;
            this.Name = name;
            this.fps =fps;
            this.ID = id;

            this.client = new CIPC_CS.CLIENT.CLIENT(myport, serverip, serverport, name, fps);
        }

        public void Setup(CIPC_CS.CLIENT.MODE mode)
        {
            try
            {
                this.client.Setup(mode);
                this.client.DataReceived += client_DataReceived;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Send(ref byte[] data)
        {
            this.client.Update(ref data);
        }

        public void Close()
        {
            this.client.Close();
        }

        void client_DataReceived(object sender, byte[] e)
        {
            this.OnDataReceived(e);
        }

        public delegate void DataReceivedEventHandler(object sender, byte[] e);
        public event DataReceivedEventHandler DataReceived;
        protected virtual void OnDataReceived(byte[] e)
        {
            if (this.DataReceived != null) { this.DataReceived(this, e); }
        }

    }
}
