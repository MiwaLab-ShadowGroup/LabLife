using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LabLife.Editor
{
    /// <summary>
    /// CIPCを操作して同期用信号の送信を行います
    /// </summary>
    public class SyncRecordManagerCIPCPanel : ADefaultPanel
    {
        public CIPC_CS.CLIENT.CLIENT client;

        public SyncRecordManagerCIPCPanel()
        {
            this.TitleName = "SyncRecordManagerCIPCPanel";
        }

        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);
            
        }
        public override void Close(object sender, RoutedEventArgs e)
        {
            base.Close(sender, e);
        }

    }
}
