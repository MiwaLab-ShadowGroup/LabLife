using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.NeuralProcesser.Internal
{
    public class Energy
    {
        private double m_value;
        public bool IsLeaving
        {
            get
            {
                return m_value > 0 ? true : false;
            }
        }

        public Energy(double Value)
        {
            this.m_value = Value;
        }
        public void Add(double value)
        {
            this.m_value += value;
        }
        public void Use(double value)
        {
            this.m_value -= value;
        }
    }
}
