using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace LabLife.Editor
{
    public class WindowListPanel : ADefaultPanel
    {
        public TreeView treeview = new TreeView();
        public WindowListPanel()
        {
            this.TitleName = "Window List";
            this.Children.Add(this.treeview);
        }
        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            foreach (var p in mainwindow.PanelList)
            {
                var item = new TreeViewItem();
                item.Header = p.TitleName;
                foreach (var q in p.Children)
                {
                    item.Items.Add(q.ToString());
                }
                this.treeview.Items.Add(item);
            }
            this.AddContent(this.treeview, Dock.Top);

        }
    }
}
