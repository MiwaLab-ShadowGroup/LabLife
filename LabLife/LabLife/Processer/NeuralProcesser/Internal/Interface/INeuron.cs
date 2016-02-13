using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.NeuralProcesser.Internal.Interface
{
    public enum NeuronMode
    {
        Input,
        Output,
        Normal
    }
    public enum ExcitationState
    {
        High,
        Low,
        Other
    }
    
    /// <summary>
    /// 素子としての基礎
    /// </summary>
    public interface INeuron : IBasicNeuron
    {
        void AddEnergy(double value);
        void Update();
        void Die();
        NeuronMode GetMode();
    }
}
