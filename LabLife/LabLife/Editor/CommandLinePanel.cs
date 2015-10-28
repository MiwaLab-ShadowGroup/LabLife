using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LabLife.Editor
{
    public class CommandLinePanel : DefaultPanel
    {
        private static List<string> LineList = new List<string>();


        public static List<string> getInstance()
        {
            return LineList;
        }

        public TextBox TextBox_Command=new TextBox();
        public TextBlock TextBlock_Log = new TextBlock();
        public ScrollViewer ScrollViewer_TextLog = new ScrollViewer();


        public StreamReader m_StandardOutputCmd;
        public StreamWriter m_StandardInputCmd;
        private Process m_Cmd;

        public CommandLinePanel()
        {
            this.TitleName = "Command Line";
        }

        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);


            this.m_Cmd = new System.Diagnostics.Process();

            //ComSpec(cmd.exe)のパスを取得して、FileNameプロパティに指定
            this.m_Cmd.StartInfo.FileName = System.Environment.GetEnvironmentVariable("ComSpec");
            //出力を読み取れるようにする
            this.m_Cmd.StartInfo.UseShellExecute = false;
            this.m_Cmd.StartInfo.RedirectStandardOutput = true;
            this.m_Cmd.StartInfo.RedirectStandardError = true;
            this.m_Cmd.StartInfo.RedirectStandardInput = true;
            this.m_Cmd.StartInfo.CreateNoWindow = true;
            //ウィンドウを表示しないようにする
            //コマンドラインを指定（"/c"は実行後閉じるために必要）
            this.m_Cmd.StartInfo.Arguments = @"dir c:\ /w";
            this.m_Cmd.OutputDataReceived += M_Cmd_OutputDataReceived;
            this.m_Cmd.ErrorDataReceived += M_Cmd_OutputDataReceived;
            //起動
            this.m_Cmd.Start();
            //this.m_StandardOutputCmd = this.m_Cmd.StandardOutput;
            this.m_Cmd.BeginOutputReadLine();
            this.m_Cmd.BeginErrorReadLine();


            this.m_StandardInputCmd = this.m_Cmd.StandardInput;

            this.ScrollViewer_TextLog.Content = this.TextBlock_Log;
            this.ScrollViewer_TextLog.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.ScrollViewer_TextLog.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            this.TextBox_Command.KeyDown += TextBox_Command_KeyDown;
            this.TextBlock_Log.VerticalAlignment = VerticalAlignment.Top;
            this.AddContent(this.TextBox_Command, Dock.Top);
            this.AddContent(ScrollViewer_TextLog, Dock.Top);
        }

        private void TextBox_Command_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                if(this.TextBox_Command.Text == "exit")
                {
                    base.m_MainWindow.Close();
                    return;
                }

                if (this.TextBox_Command.Text == "clear")
                {
                    this.TextBlock_Log.Text = "";
                    this.TextBox_Command.Text = "";
                    return;
                }
                this.m_StandardInputCmd.WriteLine(this.TextBox_Command.Text);
                this.TextBox_Command.Text = "";
            }

            if(e.Key == System.Windows.Input.Key.Up)
            {
                this.ScrollViewer_TextLog.LineUp();
            }
            if (e.Key == System.Windows.Input.Key.Down)
            {
                this.ScrollViewer_TextLog.LineDown();
            }
        }

        private void M_Cmd_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.TextBlock_Log.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.TextBlock_Log.Text += e.Data + "\n";
                this.ScrollViewer_TextLog.ScrollToEnd();
            }));
        }

        public override void Close(object sender, RoutedEventArgs e)
        {
            base.Close(sender, e);
            m_Cmd.Close();
        }
    }
}
