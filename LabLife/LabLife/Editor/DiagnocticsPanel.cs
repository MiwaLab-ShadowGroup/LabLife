using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Media;

namespace LabLife.Editor
{
    public class DiagnocticsPanel : DefaultPanel
    {
        public TreeView treeview = new TreeView();
        public ListBox ListBox_ProcessList = new ListBox();
        public List<Process> ProcessList = new List<Process>();
        public DiagnocticsPanel()
        {
            this.TitleName = "Diagnoctics";
            this.Children.Add(this.ListBox_ProcessList);
        }
        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            this.ProcessList = System.Diagnostics.Process.GetProcesses().ToList();
            this.ProcessList.Sort((a,b)=> (a.ProcessName.CompareTo(b.ProcessName)));
            foreach (var p in this.ProcessList)
            {
                StackPanel st = new StackPanel();
                st.Orientation = Orientation.Horizontal;
                TextBlock title = new TextBlock();
                title.Text = p.ProcessName;
                title.Width = 300;

                TextBlock id = new TextBlock();
                id.Text = p.Id.ToString();
                id.Width = 50;

                st.Children.Add(title);
                st.Children.Add(id);
                this.ListBox_ProcessList.Items.Add (st);
            }
            TreeViewItem tvi = new TreeViewItem();
            tvi.MouseLeftButtonDown += Tvi_MouseLeftButtonDown;
            tvi.Header = "ProcessList";
            tvi.Items.Add(ListBox_ProcessList);
            this.treeview.Items.Add(tvi);
            this.AddContent(this.treeview, Dock.Top);

        }

        private void Tvi_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((TreeViewItem)sender).IsExpanded = !((TreeViewItem)sender).IsExpanded;
        }
    }
}
