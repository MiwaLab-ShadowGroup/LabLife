using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.NeuralProcesser.Internal.Interface
{
    public interface ISenderNeuron : INeuron
    {
        void ConnectTo(IReceiverNeuron target, TransferCoefficient TC);
        void Send(IReceiverNeuron receiverNeuron, INeurotransmitter neurotransmitter);
        void RemoveConnection(IReceiverNeuron target);
    }
}
