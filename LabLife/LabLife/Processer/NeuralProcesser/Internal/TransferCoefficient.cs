using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.NeuralProcesser.Internal
{
    /// <summary>
    /// 伝達（減衰）係数
    /// </summary>
    public class TransferCoefficient
    {
        /// <summary>
        /// 係数
        /// </summary>
        public double m_value { set; get; }

        public TransferCoefficient()
        {
            m_value = 0;
        }
        public TransferCoefficient(double value)
        {
            m_value = value;
            if (this.Verify())
            {
                Console.Write(this, "Invalid value", Console.LogType.Warning);
            }
        }

        public bool Verify()
        {
            return this.m_value >= 0 && this.m_value <= 1;
        }

        public Interface.INeurotransmitter Cofficient(Interface.INeurotransmitter neurotransmitter)
        {
            return neurotransmitter.TransfarAttenuation(this);
        }
    }
}
