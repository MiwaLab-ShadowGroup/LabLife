using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LabLife.Editor
{
    public class LauncherPanel : ADefaultPanel
    {
        public List<LabLife.Contorols.LauncherControl> ProcessControllList = new List<Contorols.LauncherControl>();

        public LauncherPanel()
        {
            this.TitleName = "Launcher";
        }
        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            ScrollViewer sv = new ScrollViewer();
            StackPanel stp = new StackPanel();

            this.Initialize_List();

            foreach(var p in this.ProcessControllList)
            {
                stp.Children.Add(p);
            }
            sv.Content = stp;
            this.AddContent(sv, Dock.Top);

        }

        private void Initialize_List()
        {
            this.ProcessControllList.Add(new Contorols.LauncherControl("CentralInterProcessCommunicationServer.exe", "CIPCServer", System.Windows.Media.Color.FromRgb(0, 100, 0)));
            this.ProcessControllList.Add(new Contorols.LauncherControl("StreamController.exe", "StreamController", System.Windows.Media.Color.FromRgb(0,0,100)));
            this.ProcessControllList.Add(new Contorols.LauncherControl("StreamAnalyzer.exe", "StreamAnalyzer", System.Windows.Media.Color.FromRgb(100,0,100)));
            this.ProcessControllList.Add(new Contorols.LauncherControl("CIPCTerminal.exe", "CIPCTerminal", System.Windows.Media.Color.FromRgb(100, 100, 100)));
            this.ProcessControllList.Add(new Contorols.LauncherControl("ArduinoCommunication.exe", "ArduinoCommunication", System.Windows.Media.Color.FromRgb(0, 100, 100)));

        }
    }
}
