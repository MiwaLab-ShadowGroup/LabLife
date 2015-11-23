using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace UDPDepthTest
{
    class Program
    {
        //static IplImage img;
        static DepthClient dc;
        static void Main(string[] args)
        {
            getdepth gd = new getdepth();
            gd.DataReceived += Gd_DataReceived;
            dc = new DepthClient(424, 512);
            dc.InitCLients();

            while (true)
            {
                Console.WriteLine(dc.Recieve().Length);

                
            }
        }

        private static void Gd_DataReceived(object sender, ushort[] e)
        {
            dc.Send("127.0.0.1", e);
            //192.168.11.4
            //127.0.0.1
        }
    }
}
