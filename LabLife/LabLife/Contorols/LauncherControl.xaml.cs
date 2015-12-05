using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LabLife.Contorols
{
    /// <summary>
    /// LauncherControl.xaml の相互作用ロジック
    /// </summary>
    public partial class LauncherControl : UserControl
    {
        public string m_path { set; get; }
        public string m_name { set; get; }
        public Color m_mainColor { set; get; }
        public Process m_process { set; get; }

        public LauncherControl(string Path, string Name, Color MainColor)
        {
            InitializeComponent();
            m_path = Path;
            m_name = Name;
            this.TextBlock_Title.Text = Name;
            this.Border_Main.Background = new SolidColorBrush(MainColor);
        }


        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {


            m_process = Process.Start(m_path);

        }

        private void Button_Kill_Click(object sender, RoutedEventArgs e)
        {
            try {
                if (m_process != null)
                {
                    m_process.CloseMainWindow();
                    m_process = null;
                }
            }catch(Exception ex)
            {
                Data.General.Log(this, ex.Message);
            }
        }
    }
}
