using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.NeuralProcesser.Internal.Interface
{
    /// <summary>
    /// 基本的な操作など
    /// </summary>
    public interface IBasicNeuron
    {
        void Initialize();
        ExcitationState GetCurrentState();
    }
}
