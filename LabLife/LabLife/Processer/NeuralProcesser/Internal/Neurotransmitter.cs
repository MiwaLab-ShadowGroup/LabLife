using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabLife.Processer.NeuralProcesser.Internal.Interface;

namespace LabLife.Processer.NeuralProcesser.Internal
{
    /// <summary>
    /// 伝達されるもの
    /// </summary>
    public class Neurotransmitter : Interface.INeurotransmitter
    {
        private double m_value;

        public Neurotransmitter()
        {
            m_value = 0;
        }
        public Neurotransmitter(double value)
        {
            this.m_value = value;
        }

        public List<double> getParams()
        {
            List<double> value = new List<double>();
            value.Add(this.m_value);
            return value;
        }

        public int getParamsNum()
        {
            return 1;
        }

        /// <summary>
        /// 減衰したものを返す
        /// </summary>
        /// <param name="value">減衰係数</param>
        /// <returns></returns>
        public INeurotransmitter TransfarAttenuation(TransferCoefficient value)
        {
            return new Neurotransmitter(value.m_value * m_value);
        }
    }
}
