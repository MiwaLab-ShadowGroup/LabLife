using LabLife.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace LabLife.Editor
{
    public class LogPanel : ADefaultPanel
    {
        private ScrollViewer ScrollViewer_TextLog = new ScrollViewer();
        private TextBlock TextBlock_LogMain = new TextBlock();
        public LogPanel()
        {
            this.TitleName = "Log";
        }

        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);
            base.AddContent(ScrollViewer_TextLog, Dock.Top);
            this.ScrollViewer_TextLog.Content = this.TextBlock_LogMain;
            this.TextBlock_LogMain.FontFamily = new System.Windows.Media.FontFamily("System");
            this.TextBlock_LogMain.TextWrapping = System.Windows.TextWrapping.WrapWithOverflow;
            this.ScrollViewer_TextLog.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.ScrollViewer_TextLog.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            this.TextBlock_LogMain.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            General.OnLogChange += General_OnLogChange;

        }

        private void General_OnLogChange(string data)
        {
            this.TextBlock_LogMain.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.TextBlock_LogMain.Text = data;
                this.ScrollViewer_TextLog.ScrollToEnd();
            }));
        }

        public override void Close(object sender, RoutedEventArgs e)
        {
            base.Close(sender, e);
            General.OnLogChange -= General_OnLogChange;
        }
    }
}
