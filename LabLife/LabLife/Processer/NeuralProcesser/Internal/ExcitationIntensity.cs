using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.NeuralProcesser.Internal
{
    /// <summary>
    /// Neuronの励起強度
    /// </summary>
    public class ExcitationIntensity
    {
        private double m_value;
        private double m_threathold;
        public Interface.ExcitationState State { private set; get; }

        public ExcitationIntensity()
        {

        }

        public ExcitationIntensity(double value)
        {
            this.m_value = value;
        }
        
    }
}
