using LabLife.Editor;
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
    /// Interaction logic for FloatingWindow.xaml
    /// </summary>
    public partial class FloatingWindow : Window
    {
        private LabLife.Contorols.LLCheckBox LLCheckBox_IsTopMost = new Contorols.LLCheckBox();
        public FloatingWindow()
        {
            InitializeComponent();

            this.DockPanel_Header.Children.Add(LLCheckBox_IsTopMost);
            DockPanel.SetDock(this.LLCheckBox_IsTopMost, Dock.Right);
            this.LLCheckBox_IsTopMost.Content = " ↑ ";
            this.LLCheckBox_IsTopMost.Width = 30;
            this.LLCheckBox_IsTopMost.Click += LLCheckBox_IsTopMost_Click;
            this.DockPanel_Header.Children.Add(new TextBlock());

            this.InitEvents();
        }

        private void LLCheckBox_IsTopMost_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = this.LLCheckBox_IsTopMost.IsChecked;
        }

        public void Add(DefaultPanel item)
        {
            this.DockPanel_Sub_Window_Main.Children.Add(item);
            this.TextBlock_Title.Text += " - " +item.TitleName;
            this.Title = this.TextBlock_Title.Text;
        }

        public bool hasChildren
        {
            get
            {
                if(this.DockPanel_Sub_Window_Main.Children.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public void UpdateClose()
        {
            if (!this.hasChildren)
            {
                this.Close();
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        

        private void Button_Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void InitEvents()
        {
            this.DockPanel_Header.MouseLeftButtonDown += (s, e) => this.DragMoveByHeader(s, e);
            this.Closing += FloatingWindow_Closing;
        }

        private void FloatingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DockPanel_Sub_Window_Main.Children.Clear();
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
        private void Button_Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                this.WindowState = WindowState.Normal;

            }
        }
    }
}
