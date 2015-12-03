using LabLife.Data;
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
        public List<ADefaultPanel> PanelList = new List<ADefaultPanel>();
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
            try
            {
                this.PanelList.Add(new LogPanel());
                this.PanelList.Add(new CommandLinePanel());
                this.PanelList.Add(new WindowListPanel());
                this.PanelList.Add(new DiagnocticsPanel());
                this.PanelList.Add(new ImageReceiverHostPanel());
                this.PanelList.Add(new TransporterHostPanel());
                this.PanelList.Add(new RecoderPanel());
                this.PanelList.Add(new SyncRecordManagerCIPCPanel());

                if (System.Environment.Is64BitOperatingSystem)
                {
                    this.PanelList.Add(new KinectPanel());
                }
                this.PanelList.Add(new ProjectionPanel(1));
                this.PanelList.Add(new ProjectionPanel(2));
                this.PanelList.Add(new ProjectionPanel(3));
                this.PanelList.Add(new ProjectionPanel(4));
                this.PanelList.Add(new ProjectionPanel(5));
                this.PanelList.Add(new ProjectionPanel(6));
            }
            catch (Exception ex)
            {
                General.Log(this, ex.Message);
            }

            foreach (var p in this.PanelList)
            {
                p.Initialize(this);
            }
        }

        public List<T> GetPanels<T>()
            where T : ADefaultPanel
        {
            List<T> ListPanels = new List<T>();
            foreach (var p in this.PanelList)
            {
                T item = p as T;
                if (item != null)
                {
                    ListPanels.Add(item);
                }
            }
            return ListPanels;
        }

        public ADefaultPanel GetPanel<T>()
            where T : ADefaultPanel
        {
            return this.PanelList.Find(p => p.GetType() == typeof(T));
        }

        public void AddPanel(ADefaultPanel panel)
        {
            this.PanelList.Add(panel);
            this.InitMenu();
        }
        public void RemovePanel(ADefaultPanel panel)
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
            ADefaultPanel panel = this.PanelList.Find(i => i.TitleName == (string)menuitem.Header);

            this.DisplayPanel(panel);
        }

        public void DisplayPanel(ADefaultPanel panel)
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
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var p in this.PanelList)
            {
                p.Close(sender, new RoutedEventArgs());
            }
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
