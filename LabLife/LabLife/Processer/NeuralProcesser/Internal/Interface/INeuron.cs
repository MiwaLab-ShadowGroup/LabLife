using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.NeuralProcesser.Internal.Interface
{
    public enum ConnectMode
    {
        Input,
        Output
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
        void ConnectTo(INeuron target, ConnectMode mode);
        void Update();
        void Die();
    }
}
