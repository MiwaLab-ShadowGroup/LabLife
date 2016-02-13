using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabLife.Processer.NeuralProcesser.Internal.Interface;

namespace LabLife.Processer.NeuralProcesser.Internal
{
    public class SensorNeuron<T> : Interface.ISenderNeuron
    {
        public List<KeyValuePair<IReceiverNeuron, TransferCoefficient>> m_Children;
        private ExcitationIntensity m_EI;
        private T data;

        public void AddEnergy(double value)
        {

        }

        public void ConnectTo(IReceiverNeuron target, TransferCoefficient TC)
        {
            this.m_Children.Add(new KeyValuePair<IReceiverNeuron, TransferCoefficient>(target, TC));
            target.ConnectFrom(this);
        }

        /// <summary>
        /// 死なない
        /// </summary>
        public void Die()
        {
            this.Initialize();
        }

        public ExcitationState GetCurrentState()
        {
            return this.m_EI.State;
        }

        public NeuronMode GetMode()
        {
            return NeuronMode.Input;
        }

        public void Initialize()
        {
            this.m_Children = new List<KeyValuePair<IReceiverNeuron, TransferCoefficient>>();
            this.m_EI = new ExcitationIntensity(0, 0, 1);
        }

        public void RemoveConnection(IReceiverNeuron target)
        {
            this.m_Children.RemoveAll(p => p.Key.Equals(target));
        }

        public void Send(IReceiverNeuron receiverNeuron, INeurotransmitter neurotransmitter)
        {
            receiverNeuron.Receive(neurotransmitter);
        }

        public void Update()
        {
            //伝達物質の作成
            Neurotransmitter nt = new Neurotransmitter(m_EI.Value);

            //送信
            foreach (var p in this.m_Children)
            {
                this.Send(p.Key, p.Value.Cofficient(nt));
            }

        }

        public void SetValue(T Value)
        {
            this.data = Value;
            this.m_EI = new ExcitationIntensity((double)(Value as Double?), 0, 1);
        }

    }
}
