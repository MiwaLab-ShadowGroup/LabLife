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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LabLife
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitEvents();

            this.DockPanel_MainDock.Children.Add(new Editor.WindowListPanel(this));
        }
        private void InitEvents()
        {
            this.DockPanel_Header.MouseLeftButtonDown += (s, e) => this.DragMoveByHeader(s, e);
        }

        private void DragMoveByHeader(object s, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.Top = -20;
                var p = e.GetPosition(this).X / this.ActualWidth;
                this.Left = e.GetPosition(this).X;
                this.WindowState = WindowState.Normal;
                this.Left -= p * this.Width;
            }
            this.DragMove();
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
