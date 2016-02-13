using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.NeuralProcesser
{
    public class NeuralNetwork
    {
        private List<Internal.Interface.INeuron> NeuronList;
        
        public NeuralNetwork()
        {
            this.NeuronList = new List<Internal.Interface.INeuron>();
        }
        
        public void CreateSenderNeuron(IEnumerable<object> Sender)
        {
            this.NeuronList.Add()
        }

        public void CreateReceiverNeuron(IEnumerable<object> Sender)
        {

        }
    }
}
