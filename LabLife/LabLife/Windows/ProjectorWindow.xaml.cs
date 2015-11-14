using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace LabLife.Windows
{
    /// <summary>
    /// Interaction logic for ProjectorWindow.xaml
    /// </summary>
    public partial class ProjectorWindow : Window
    {
        public ProjectorWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += (s, e) => this.DragMove();
            this.MouseDoubleClick += ProjectorWindow_MouseDoubleClick;
        }

        private void ProjectorWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(this.WindowState != WindowState.Normal)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
    }
}
