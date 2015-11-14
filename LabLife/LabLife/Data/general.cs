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
        public static void Log(object sender, string str)
        {
            log += "[ " + sender.ToString() + " ] " + str + "\n";
            if (log.Count(p=>p == '\n') >= 10)
            {
                log = log.Remove(0, log.IndexOf('\n') + 1);
            }
            if (OnLogChange != null)
            {
                OnLogChange(log);
            }
        }
    }
}
