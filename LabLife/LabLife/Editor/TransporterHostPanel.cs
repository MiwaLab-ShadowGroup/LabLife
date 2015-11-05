using LabLife.Data;
using LabLife.Processer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace LabLife.Editor
{
    public class TransporterHostPanel : ADefaultPanel
    {
        private ScrollViewer ScrollViewer_ResourcePanels = new ScrollViewer();
        private ListBox ListBox_ResourcePanels = new ListBox();

        private ScrollViewer ScrollViewer_ProjectionPanels = new ScrollViewer();
        private ListBox ListBox_ProjectionPanel = new ListBox();

        private ScrollViewer ScrollViewer_Transporters = new ScrollViewer();
        private ListBox ListBox_Transporters = new ListBox();

        public List<Transporter> List_Transporter = new List<Transporter>();
        
        public TransporterHostPanel()
        {
            this.TitleName = "Transporter";
        }

        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            UniformGrid grid = new UniformGrid();
            grid.Columns = 3;

            GroupBox gb1 = new GroupBox();
            gb1.Header = "Resource Panels";
            gb1.Content = this.ListBox_ResourcePanels;
            grid.Children.Add(gb1);
            Grid.SetColumn(gb1, 0);
            
            GroupBox gb2 = new GroupBox();
            gb2.Header = "TransPorter";
            gb2.Content= this.ListBox_Transporters;
            grid.Children.Add(gb2);
            Grid.SetColumn(gb2, 1);

            GroupBox gb3 = new GroupBox();
            gb3.Header = "Projection Panels";
            gb3.Content = this.ListBox_ProjectionPanel;
            grid.Children.Add(gb3);
            Grid.SetColumn(gb3, 2);

            StackPanel StackPanel_Control = new StackPanel();
            Button Button_Create = new Button();
            Button_Create.Click += Button_Create_Click;
            Button_Create.Content = "Add";
            Button Button_Delete = new Button();
            Button_Delete.Click += Button_Delete_Click;
            Button_Delete.Content = "Delete";

            Button Button_Update = new Button();
            Button_Update.Click += Button_Update_Click;
            Button_Update.Content = "Update";

            StackPanel_Control.Children.Add(Button_Create);
            StackPanel_Control.Children.Add(Button_Delete);
            StackPanel_Control.Children.Add(Button_Update);
            StackPanel_Control.Orientation = Orientation.Horizontal;

            base.AddContent(StackPanel_Control, Dock.Top);
            base.AddContent(grid, Dock.Top);

            this.UpdateLists();
        }

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateLists();
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            this.List_Transporter.RemoveAt(this.ListBox_Transporters.SelectedIndex);
        }

        private void Button_Create_Click(object sender, RoutedEventArgs e)
        {
            if (this.ListBox_ResourcePanels.SelectedIndex < 0)
            {
                return;
            }
            if (this.ListBox_ProjectionPanel.SelectedIndex < 0)
            {
                return;
            }

            var item = new Transporter(
                    base.m_MainWindow.GetPanels<AImageResourcePanel>()[this.ListBox_ResourcePanels.SelectedIndex],
                    base.m_MainWindow.GetPanels<ProjectionPanel>()[this.ListBox_ProjectionPanel.SelectedIndex]
                );

            this.List_Transporter.Add(item);
            this.ListBox_Transporters.Items.Add(item.ToString());
        }

        public void UpdateLists()
        {
            var resourcePanels = base.m_MainWindow.GetPanels<AImageResourcePanel>();
            var projectionPanels = base.m_MainWindow.GetPanels<ProjectionPanel>();

            if(resourcePanels != null)
            {
                this.ListBox_ResourcePanels.Items.Clear();
                foreach(var p in resourcePanels)
                {
                    this.ListBox_ResourcePanels.Items.Add(p.TitleName);
                }
                this.ListBox_ResourcePanels.SelectedIndex = 0;
            }
            
            if (projectionPanels != null)
            {
                this.ListBox_ProjectionPanel.Items.Clear();
                foreach (var p in projectionPanels)
                {
                    this.ListBox_ProjectionPanel.Items.Add(p.TitleName);
                }
                this.ListBox_ProjectionPanel.SelectedIndex = 0;
            }
        }
    }
}
