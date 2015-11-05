﻿using LabLife.Data;
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
        private AImageResourcePanel m_srcPanel;
        private ProjectionPanel m_dstPanel;
        private int m_imageIndex;
        public Transporter(AImageResourcePanel srcPanel, int ImageIndex, ProjectionPanel dstPanel)
        {
            this.m_srcPanel = srcPanel;
            this.m_dstPanel = dstPanel;
            this.m_imageIndex = ImageIndex;
            this.m_srcPanel.ImageFrameArrived += M_srcPanel_ImageFrameArrived;
        }

        private void M_srcPanel_ImageFrameArrived(object Sender, ImageFrameArrivedEventArgs e)
        {
            General.Log(this, e.Image[this.m_imageIndex].ToString());

        }
        
        public void Dispose()
        {
            this.m_srcPanel.ImageFrameArrived -= M_srcPanel_ImageFrameArrived;
        }

        public override string ToString()
        {
            return "{ " + this.m_imageIndex +" of " + m_srcPanel.TitleName + " }  { " + m_dstPanel.TitleName + " }";
        }
    }
}
