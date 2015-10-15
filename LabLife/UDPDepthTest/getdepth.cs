using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Kinect;

namespace UDPDepthTest
{
    public class getdepth
    {

        KinectSensor kinect;

        DepthFrameReader depthFrameReader;
        FrameDescription depthFrameDescription;


        int depthStride;
        ushort[] depthBuffer;
        int depthImageWidth;
        int depthImageHeight;


        //コンストラクタ
        public getdepth()
        {
            try {
                kinect = KinectSensor.GetDefault();

                this.depthFrameReader = this.kinect.DepthFrameSource.OpenReader();
                this.depthFrameReader.FrameArrived += DepthFrame_Arrived;
                this.depthFrameDescription = this.kinect.DepthFrameSource.FrameDescription;
                this.depthBuffer = new ushort[this.depthFrameDescription.LengthInPixels];
                this.depthImageWidth = this.depthFrameDescription.Width;
                this.depthImageHeight = this.depthFrameDescription.Height;
                this.depthStride = (int)(depthFrameDescription.Width * depthFrameDescription.BytesPerPixel);


                this.kinect.Open();
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public delegate void DepthImageEventHandler(object sender, ushort[] e);
        public event DepthImageEventHandler DataReceived;
        protected virtual void OnDataReceived(ushort[] e)
        {
            if (this.DataReceived != null) { this.DataReceived(this, e); }
        }


        public void DepthFrame_Arrived(object sender, DepthFrameArrivedEventArgs e)
        {

            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {

                //フレームがなければ終了、あれば格納
                if (depthFrame == null) return;
                int[] depthBitdata = new int[depthBuffer.Length];
                depthFrame.CopyFrameDataToArray(this.depthBuffer);

                // 0～8000 を 0～65535にする（16bit）
                for (int i = 0; i < depthBuffer.Length; i++)
                {
                    depthBuffer[i] = (ushort)(depthBuffer[i] * 65535 / 8000);
                }

                OnDataReceived(depthBuffer);
            }

        }

    }

}
