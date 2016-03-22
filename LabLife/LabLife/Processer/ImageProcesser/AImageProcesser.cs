using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.ImageProcesser
{
    public enum ParameterType
    {
        Float,
        Double,
        Int,
        UInt,
        Byte,
        String,
        Other
    }
    public class Parameter
    {
        public Parameter(ParameterType Type, Object Value)
        {
            this.Type = Type;
            this.Value = Value;
        }
        public ParameterType Type { set; get; }
        public Object Value { set; get; }
    }

    public abstract class AImageProcesser
    {
        public abstract void ImageProcess(ref Mat src, ref Mat dst);
        protected AImageProcesser()
        {
            m_parameter = new List<Parameter>();

        }
        /// <summary>
        /// UI使用するパラメータ取得
        /// </summary>
        /// <returns></returns>
        public List<Parameter> getParameter()
        {
            return this.m_parameter;
        }
        private List<Parameter> m_parameter;
    }
}
