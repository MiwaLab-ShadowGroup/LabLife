using LabLife.Editor;
using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer
{
    public class Transporter : IDisposable
    {
        private AImageResourcePanel srcPanel;
        private ProjectionPanel dstPanel;
        public Transporter(AImageResourcePanel srcPanel, ProjectionPanel dstPanel)
        {
            this.srcPanel = srcPanel;
            this.dstPanel = dstPanel;

            this.srcPanel.ImageFrameArrived += SrcPanel_ImageFrameArrived;
        }

        private void SrcPanel_ImageFrameArrived(object Sender, EventArgs e)
        {
            
        }
        
        public void Dispose()
        {
            this.srcPanel.ImageFrameArrived -= SrcPanel_ImageFrameArrived;
        }

        public override string ToString()
        {
            return "{ " + srcPanel.TitleName + " }  { " + dstPanel.TitleName + " }";
        }
    }
}
