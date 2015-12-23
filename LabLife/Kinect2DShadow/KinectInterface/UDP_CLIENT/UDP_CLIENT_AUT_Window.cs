using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UDP
{
    public partial class UDP_CLIENT_AUT_Window : Form
    {
        UDP_CLIENT_AUT client;
        public event _DataReceived DataReceived;

        public UDP_CLIENT_AUT_Window()
        {
            InitializeComponent();
            this.button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.client == null)
            {
                int myPort = int.Parse(this.textBox1.Text);
                string remoteIP = this.textBox2.Text;
                int remotePort = int.Parse(this.textBox3.Text);
                this.client = new UDP_CLIENT_AUT();
                this.client.DataReceived += this.DataReceived;
                this.client.makenewclient += this.NewClient;
                this.button2.Enabled = true;
                this.button1.Text = "disconnect";
            }
            else
            {
                this.client.Close();
                this.button2.Enabled = false;
                this.button1.Text = "Connect";

            }
        }

        public void Send(byte[] data)
        {
            try
            {
                if (this.client != null)
                    this.client.Send(data);
            }
            catch
            {

            }
            
        }

        void NewClient(int m, int c)
        {
            this.textBox1.BeginInvoke(new Action(() => this.textBox1.Text = m.ToString()));
            this.textBox3.BeginInvoke(new Action(() => this.textBox3.Text = c.ToString()));
            this.button2.BeginInvoke(new Action(() => this.button2.Enabled = false));
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if(this.client != null)
                this.client.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.client.Access();
        }


    }
}
