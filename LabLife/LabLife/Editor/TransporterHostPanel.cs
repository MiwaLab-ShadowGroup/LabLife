using LabLife.Data;
using LabLife.Processer;
using System;
using System.Collections.Generic;
using System.IO;
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

        public List<FileInfo> CallibrationDataPathList = new List<FileInfo>();
        private ListBox ListBox_CallbPathList = new ListBox();
        public TextBox TextBox_SaveFileName = new TextBox();
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
            b2.Height = 150;
            b2.Style = (Style)App.Current.Resources["Border_Default"];
            b2.Child = this.ListBox_Transporters;
            Dock.Children.Add(b2);
            DockPanel.SetDock(b2, System.Windows.Controls.Dock.Top);

            Border b4 = new Border();
            b4.Style = (Style)App.Current.Resources["Border_Default"];
            StackPanel stp = new StackPanel();
            ScrollViewer sv = new ScrollViewer();
            b4.Child = sv;
            sv.Content = stp;
            UniformGrid uniformgrid_header = new UniformGrid();
            uniformgrid_header.Columns = 3;
            stp.Children.Add(uniformgrid_header);

            uniformgrid_header.Children.Add(this.TextBox_SaveFileName);
            Button Button_SaveCallib = new Button();
            Button_SaveCallib.Content = "SaveCallib";
            Button_SaveCallib.Click += Button_SaveCallib_Click;
            uniformgrid_header.Children.Add(Button_SaveCallib);

            Button Button_LoadCallib = new Button();
            Button_LoadCallib.Content = "LoadCallib";
            Button_LoadCallib.Click += Button_LoadCallib_Click;
            uniformgrid_header.Children.Add(Button_LoadCallib);

            stp.Children.Add(this.ListBox_CallbPathList);
            Dock.Children.Add(b4);
            DockPanel.SetDock(b4, System.Windows.Controls.Dock.Bottom);


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

        private void Button_LoadCallib_Click(object sender, RoutedEventArgs e)
        {
            int index = this.ListBox_Transporters.SelectedIndex;
            if (index < 0 || index >= this.ListBox_Transporters.Items.Count)
            {
                return;
            }
            int callibindex = this.ListBox_CallbPathList.SelectedIndex;
            if (index < 0 || callibindex >= this.ListBox_CallbPathList.Items.Count)
            {
                return;
            }
            
            Data.CallibrationData callibdata = new CallibrationData();
            callibdata.LoadCallibration(
                this.CallibrationDataPathList[this.ListBox_CallbPathList.SelectedIndex].FullName,
                this.List_Transporter[index].src,
                this.List_Transporter[index].dst
                );
            this.List_Transporter[index].SetupWrap(this.List_Transporter[index].src,
                this.List_Transporter[index].dst);
        }

        void Button_SaveCallib_Click(object sender, RoutedEventArgs e)
        {
            int index = this.ListBox_Transporters.SelectedIndex;
            if (index < 0 || index >= this.ListBox_Transporters.Items.Count)
            {
                return;
            }
            Data.CallibrationData callibdata = new CallibrationData();
            callibdata.SaveCallibration(
                LabLifeSettings.CallibrationPath + @"\" + this.TextBox_SaveFileName.Text + ".callib",
                this.List_Transporter[index].src,
                this.List_Transporter[index].dst
                );
            this.UpdateLists();
        }

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            this.UpdateLists();
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (this.ListBox_Transporters.SelectedIndex < 0)
            {
                return;
            }
            this.List_Transporter[this.ListBox_Transporters.SelectedIndex].Dispose();
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
                    Grid.GetColumn(this.Dictionary_ResourceImage.ElementAt(this.ListBox_ResourcePanels.SelectedIndex).Key),
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

            this.UpdateTransportList();
            this.UpdateTransportListBox();
        }



        private void UpdateTransportList()
        {
            this.CallibrationDataPathList.Clear();
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(LabLifeSettings.CallibrationPath);
            var files = di.GetFiles("*.callib");
            foreach (var p in files)
            {
                this.CallibrationDataPathList.Add(p);
            }
        }

        private void UpdateTransportListBox()
        {
            this.ListBox_CallbPathList.Items.Clear();
            foreach (var p in this.CallibrationDataPathList)
            {
                this.ListBox_CallbPathList.Items.Add(p.Name);
            }
        }
    }
}
