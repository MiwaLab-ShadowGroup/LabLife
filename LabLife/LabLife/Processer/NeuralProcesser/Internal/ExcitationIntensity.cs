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
        private double m_reduceIntencity;


        public double Value
        {
            get
            {
                return this.m_value;
            }
        }
        public double Threathold
        {
            get
            {
                return this.m_threathold;
            }
        }

        public Interface.ExcitationState State
        {
            get { return this.Value > this.Threathold ? Interface.ExcitationState.High : Interface.ExcitationState.Low; }
        }

        public ExcitationIntensity(double value, double threathold, double ReducalIntencity)
        {
            this.Reset();
            this.m_value = value;

            this.m_threathold = threathold;


            this.m_reduceIntencity = ReducalIntencity;
        }

        /// <summary>
        /// 興奮物質が来る
        /// </summary>
        /// <param name="value"></param>
        public void add(double value)
        {
            this.m_value += value;
        }
        public void ResetValue()
        {
            this.m_value = 0;
        }

        /// <summary>
        /// 初期値に戻す
        /// </summary>
        public void ResetThreathold()
        {
            this.m_threathold = 0;
        }

        public void Reduce()
        {
            this.m_value *= m_reduceIntencity;
        }

        /// <summary>
        /// Reset all value;
        /// </summary>
        public void Reset()
        {
            this.ResetThreathold();
            this.ResetValue();
        }

    }
}
