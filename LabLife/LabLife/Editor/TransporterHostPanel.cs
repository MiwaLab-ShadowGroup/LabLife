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

        public Dictionary<Image, AImageResourcePanel> Dictionary_ResourceImage = new Dictionary<Image, AImageResourcePanel>();

        public List<Transporter> List_Transporter = new List<Transporter>();

        public TransporterHostPanel()
        {
            this.TitleName = "Transporter";
        }

        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            DockPanel Dock = new DockPanel();
            Border b1 = new Border();
            b1.Style = (Style)App.Current.Resources["Border_Default"];
            b1.Child = this.ListBox_ResourcePanels;
            Dock.Children.Add(b1);
            DockPanel.SetDock(b1, System.Windows.Controls.Dock.Left);

            this.ListBox_ResourcePanels.MinWidth = 100;
            this.ListBox_ProjectionPanel.MinWidth = 100;
            this.ListBox_Transporters.MinWidth = 100;



            Border b3 = new Border();
            b3.Style = (Style)App.Current.Resources["Border_Default"];
            b3.Child = this.ListBox_ProjectionPanel;
            Dock.Children.Add(b3);
            DockPanel.SetDock(b3, System.Windows.Controls.Dock.Right);


            Border b2 = new Border();
            b2.Style = (Style)App.Current.Resources["Border_Default"];
            b2.Child = this.ListBox_Transporters;
            Dock.Children.Add(b2);
            DockPanel.SetDock(b2, System.Windows.Controls.Dock.Left);

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

            base.AddContent(StackPanel_Control, System.Windows.Controls.Dock.Top);
            base.AddContent(Dock, System.Windows.Controls.Dock.Top);

            this.UpdateLists();
        }

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateLists();
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if(this.ListBox_Transporters.SelectedIndex < 0)
            {
                return;
            }
            this.List_Transporter.RemoveAt(this.ListBox_Transporters.SelectedIndex);
            this.ListBox_Transporters.Items.RemoveAt(this.ListBox_Transporters.SelectedIndex);
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
                    this.Dictionary_ResourceImage.ElementAt(this.ListBox_ResourcePanels.SelectedIndex).Value,
                    Grid.GetColumn( this.Dictionary_ResourceImage.ElementAt(this.ListBox_ResourcePanels.SelectedIndex).Key),
                    base.m_MainWindow.GetPanels<ProjectionPanel>()[this.ListBox_ProjectionPanel.SelectedIndex]
                );

            this.List_Transporter.Add(item);
            this.ListBox_Transporters.Items.Add(item.ToString());
        }

        public void UpdateLists()
        {
            var resourcePanels = base.m_MainWindow.GetPanels<AImageResourcePanel>();
            var projectionPanels = base.m_MainWindow.GetPanels<ProjectionPanel>();

            if (resourcePanels != null)
            {
                this.ListBox_ResourcePanels.Items.Clear();
                this.Dictionary_ResourceImage.Clear();
                foreach (var p in resourcePanels)
                {
                    for (int i = 0; i < p.ImageNum; i++)
                    {
                        this.ListBox_ResourcePanels.Items.Add(p.TitleName + " " + i.ToString());
                        this.Dictionary_ResourceImage.Add((Image)p.Grid_Image.Children[i], p);
                    }
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
