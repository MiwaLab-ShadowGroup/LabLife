using LabLife.Contorols;
using LabLife.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LabLife.Editor
{
    public abstract class DefaultPanel : Border
    {
        private DockPanel DockPanel_Main = new DockPanel();
        private Border Border_Header = new Border();
        private DockPanel DockPanel_Header = new DockPanel();
        private TextBlock TextBlock_Header = new TextBlock();
        private Button Button_Close = new Button();
        private Button Button_ToMainWindow = new Button();
        private LLCheckBox LLCheckbox_IsClip = new LLCheckBox();

        public List<UIElement> Children = new List<UIElement>();

        public string TitleName = "";

        protected MainWindow m_MainWindow;
        
        protected void AddContent(UIElement item, Dock dock)
        {
            this.DockPanel_Main.Children.Add(item);
            DockPanel.SetDock(item, dock);
        }

        public virtual void Initialize(MainWindow mainwindow)
        {
            DockPanel.SetDock(this.Button_Close, Dock.Right);
            DockPanel.SetDock(this.TextBlock_Header, Dock.Left);
            DockPanel.SetDock(this.LLCheckbox_IsClip, Dock.Right);
            DockPanel.SetDock(this.Button_ToMainWindow, Dock.Right);

            this.Button_Close.Content = " ✕ ";
            this.Button_Close.Style = (Style)App.Current.Resources["WindowButtonStyle"];
            this.Button_Close.Click += this.Close;
            this.Button_Close.ToolTip = "Close";

            this.Button_ToMainWindow.Content = " → ";
            this.Button_ToMainWindow.Style = (Style)App.Current.Resources["WindowButtonStyle"];
            this.Button_ToMainWindow.Click += this.ToMainWindow;
            this.Button_ToMainWindow.ToolTip = "ToMainWindow";

            this.LLCheckbox_IsClip.Content = " +++ ";
            this.LLCheckbox_IsClip.ToolTip = "Fix";

            this.TextBlock_Header.Text = this.TitleName;

            this.DockPanel_Header.Children.Add(this.Button_Close);
            this.DockPanel_Header.Children.Add(this.Button_ToMainWindow);
            this.DockPanel_Header.Children.Add(this.LLCheckbox_IsClip);
            this.DockPanel_Header.Children.Add(this.TextBlock_Header);
            this.DockPanel_Header.Background = Brushes.Transparent;
            this.DockPanel_Header.MouseLeftButtonDown += DockPanel_Header_MouseLeftButtonDown; ;

            this.Border_Header.Background = new SolidColorBrush(Color.FromArgb(20,255,255,255));
            this.Border_Header.BorderThickness = new Thickness(0);
            this.Border_Header.Child = this.DockPanel_Header;
            DockPanel.SetDock(Border_Header, Dock.Top);

            this.DockPanel_Main.Children.Add(this.Border_Header);
            this.Child = DockPanel_Main;

            this.m_MainWindow = mainwindow;
            this.Style = (Style)App.Current.Resources["Border_Default"];
        }

        private void ToMainWindow(object sender, RoutedEventArgs e)
        {
            if (this.LLCheckbox_IsClip.IsChecked == true) return;
            var parent = (Panel)this.Parent;
            var window = Window.GetWindow(this);
            parent.Children.Remove(this);

            this.m_MainWindow.DockPanel_MainDock.Children.Add(this);
            if (window.GetType() == typeof(FloatingWindow))
            {
                ((FloatingWindow)window).UpdateClose();
            }
        }

        public virtual void Close(object sender, RoutedEventArgs e)
        {
            if (this.LLCheckbox_IsClip.IsChecked == true) return;
            var parent = (Panel)this.Parent;
            var window = Window.GetWindow(this);
            parent.Children.Remove(this);

            if (window.GetType() == typeof(FloatingWindow))
            {
                ((FloatingWindow)window).UpdateClose();
            }
        }

        private void DockPanel_Header_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.LLCheckbox_IsClip.IsChecked == true) return;
            Console.WriteLine("Pressed");

            var parent = (Panel)this.Parent;

            var height = this.ActualHeight + 44;
            var width = this.ActualWidth + 14;

            

            Window OwnerWindow;
            OwnerWindow = Window.GetWindow(this);

            var posx = e.GetPosition(OwnerWindow).X / OwnerWindow.ActualWidth;
            var posy = e.GetPosition(OwnerWindow).Y / OwnerWindow.ActualHeight;

            parent.Children.Remove(this);
            Windows.FloatingWindow p = new Windows.FloatingWindow();
            p.Style = (Style)App.Current.Resources["WindowStyle"];
            p.Add(this);
            p.Show();

            p.Height = height;
            p.Width = width;

            p.Left = OwnerWindow.Left;
            p.Top = OwnerWindow.Top;
            if (e.MouseDevice.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                p.DragMove();
            }
            else
            {
                Console.WriteLine(e.LeftButton.ToString());
            }
            if (OwnerWindow.GetType() == p.GetType())
            {
                Windows.FloatingWindow v = (Windows.FloatingWindow)OwnerWindow;
                v.UpdateClose();
            }
        }
    }
}
