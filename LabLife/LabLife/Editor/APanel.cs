using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LabLife.Editor
{
    public abstract class APanel:Panel
    {
        private Border Border_Outer = new Border();
        private StackPanel StackPanel_Main = new StackPanel();
        private DockPanel DockPanel_Header = new DockPanel();
        private TextBlock TextBlock_Header = new TextBlock();
        private Button Button_Close = new Button();
        public void Initialize(MainWindow mainwindow)
        {
            this.Children.Add(Border_Outer);
            this.Border_Outer.Child = StackPanel_Main;
            this.StackPanel_Main.Children.Add(this.DockPanel_Header);
            this.DockPanel_Header.Children.Add(this.TextBlock_Header);
            DockPanel.SetDock(this.TextBlock_Header, Dock.Left);
            this.TextBlock_Header.Text = "Title";
            this.Button_Close.Template = (ControlTemplate)mainwindow.Resources["WindowButtonTemplate"];
            this.Button_Close.Content = " ✕ ";
        }
        
    }
}
