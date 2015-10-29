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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LabLife
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<DefaultPanel> PanelList = new List<DefaultPanel>();
        public MainWindow()
        {
            InitializeComponent();

            InitWindow();
            InitEvents();
            InitPanelList();
            InitMenu();
        }

        private void InitWindow()
        {
            this.Title = "LabLife!";
            this.TextBlock_Title.Text = this.Title;
        }

        private void InitPanelList()
        {
            this.PanelList.Add(new CommandLinePanel());
            this.PanelList.Add(new WindowListPanel());
            this.PanelList.Add(new DiagnocticsPanel());
            this.PanelList.Add(new ImageReceiverHostPanel());


            foreach (var p in this.PanelList)
            {
                p.Initialize(this);
            }
        }

        public void AddPanel(DefaultPanel panel)
        {
            this.PanelList.Add(panel);
            this.InitMenu();
        }
        public void RemovePanel(DefaultPanel panel)
        {
            this.PanelList.Remove(panel);
            this.InitMenu();
        }

        private void InitMenu()
        {
            this.MenuItem_Window.Items.Clear();
            foreach (var p in this.PanelList)
            {
                MenuItem item = new MenuItem();
                item.Header = p.TitleName;
                item.Click += Item_Click;
                this.MenuItem_Window.Items.Add(item);
            }
        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            var menuitem = (MenuItem)sender;
            DefaultPanel panel = this.PanelList.Find(i => i.TitleName == (string)menuitem.Header);

            this.DisplayPanel(panel);
        }

        public void DisplayPanel(DefaultPanel panel)
        {
            if (Window.GetWindow(panel) == null)
            {
                this.DockPanel_MainDock.Children.Add(panel);
                DockPanel.SetDock(panel, Dock.Left);
            }
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
