using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Processer.NeuralProcesser.Internal
{
    public class Console
    {
        public enum LogType
        {
            Log,
            Caution,
            Warning,
            Error,
        }
        /// <summary>
        /// プラットフォーム事に変更する
        /// </summary>
        /// <param name="where"></param>
        /// <param name="content"></param>
        /// <param name="logType"></param>
        public static void Write(object where, string content, LogType logType)
        {
            Data.General.Log(where, "[" + logType.ToString() + "]" + content);
        }
    }
}
