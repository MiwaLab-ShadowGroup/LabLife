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
    public partial class UDP_SERVER_Window : Form
    {
        UDP_SERVER server;

        public UDP_SERVER_Window()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.server == null)
            {
                this.server = new UDP_SERVER();
                this.server.makelocalclient += this.MakeLicalClient;
                this.button1.Enabled = false;
            }
        }

        public void MakeLicalClient(int clientport, int localport, string ipadress)
        {
            ListViewItem item0 = new ListViewItem(ipadress);
            //ListViewItem item1 = new ListViewItem(clientport.ToString());
            //ListViewItem item2 = new ListViewItem(localport.ToString());
            item0.SubItems.Add(clientport.ToString());
            item0.SubItems.Add(localport.ToString());
            
           
            this.listView1.BeginInvoke(new Action(() => {this.listView1.Items.Add(item0);/*this.listView1.Items.Add(item1);this.listView1.Items.Add(item2);*/}));
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if(this.server != null)
                this.server.Close();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
