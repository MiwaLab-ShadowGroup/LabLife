using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabLife.Data
{
    public class General
    {
        public delegate void ChangeLogHandler(string data);
        public static event ChangeLogHandler OnLogChange;

        private static string log = "";
        public static void Log(object sender,string str)
        {
            log += "[ " + sender.ToString() + " ] " + str + "\n";
            if (OnLogChange != null)
            {
                OnLogChange(log);
            }
        }
    }
}
