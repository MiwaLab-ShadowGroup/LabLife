using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LRFsync
{
    
    public class LRFInfomation
    {
        public int Id;
        public CIPCClient.ReceiveClient client { get; set;}

        public List<System.Windows.Point> listLRFdata;


        public LRFInfomation()
        {

            listLRFdata = new List<System.Windows.Point>();
        }

        
    }
}
