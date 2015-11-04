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
            grid.Children.Add(this.ListBox_ResourcePanels);
            Grid.SetColumn(this.ListBox_ResourcePanels, 0);
            grid.Children.Add(this.ListBox_Transporters);
            Grid.SetColumn(this.ListBox_Transporters, 1);
            grid.Children.Add(this.ListBox_ProjectionPanel);
            Grid.SetColumn(this.ListBox_ProjectionPanel, 2);

            base.AddContent(grid, Dock.Top);
        }

        public void UpdateLists()
        {
        }
    }
}
