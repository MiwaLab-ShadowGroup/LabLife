using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.NeuralProcesser.Internal.Interface
{
    public interface IReceiverNeuron : INeuron
    {
        void Receive(INeurotransmitter neurotransmitter);
    }
}
