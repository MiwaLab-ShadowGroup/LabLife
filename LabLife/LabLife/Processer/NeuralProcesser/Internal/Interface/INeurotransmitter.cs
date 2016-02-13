using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.NeuralProcesser.Internal.Interface
{
    /// <summary>
    /// 伝達物質
    /// </summary>
    public interface INeurotransmitter
    {
        /// <summary>
        /// どのくらい減衰するか
        /// </summary>
        /// <param name="value">減衰係数</param>
        /// <returns></returns>
        INeurotransmitter TransfarAttenuation(TransferCoefficient value);
        List<double> getParams();
        int getParamsNum();
    }
}
