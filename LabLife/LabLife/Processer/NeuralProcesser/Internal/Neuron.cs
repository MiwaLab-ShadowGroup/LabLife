﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LabLife.Processer.NeuralProcesser.Internal.Interface;

namespace LabLife.Processer.NeuralProcesser.Internal
{
    public class Neuron : Interface.IReceiverNeuron, Interface.ISenderNeuron
    {
        public List<KeyValuePair<IReceiverNeuron, TransferCoefficient>> m_Children;

        /// <summary>
        /// 
        /// </summary>
        private ExcitationIntensity m_EI;

        public Neuron()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            this.m_Children = new List<KeyValuePair<IReceiverNeuron, TransferCoefficient>>();
            this.m_EI = new ExcitationIntensity();
        }


        /// <summary>
        /// 受け取ったものを各子供に送信
        /// </summary>
        /// <param name="neurotransmitter"></param>
        public void Receive(INeurotransmitter neurotransmitter)
        {
            foreach (var p in this.m_Children)
            {
                this.Send(p.Key, neurotransmitter.TransfarAttenuation(p.Value));
            }
        }

        public void ConnectTo(IReceiverNeuron target)
        {
            TransferCoefficient TC = new TransferCoefficient();
            this.m_Children.Add(new KeyValuePair<IReceiverNeuron, TransferCoefficient>(target, TC));
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void Die()
        {
            throw new NotImplementedException();
        }


        public ExcitationState GetCurrentState()
        {
            throw new NotImplementedException();
        }

        public void Send(IReceiverNeuron receiverNeuron, INeurotransmitter neurotransmitter)
        {
            receiverNeuron.Receive(neurotransmitter);
        }

    }
}
