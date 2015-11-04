using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LabLife.Editor
{
    public class ImageReceiverHostPanel : ADefaultPanel
    {
        public List<ImageReceiverPanel> List_ImageReceiver = new List<ImageReceiverPanel>();

        public ListBox ListBox_ReceiverHostPanel = new ListBox();

        public static int StartPortNum = 15000;

        public ImageReceiverHostPanel()
        {
            this.TitleName = "Image Receiver Host";
        }
        public override void Initialize(MainWindow mainwindow)
        {
            base.Initialize(mainwindow);

            Border bd = new Border();
            bd.Child = this.ListBox_ReceiverHostPanel;
            bd.Style = (Style)App.Current.Resources["Border_Default"];
            
            this.AddContent(bd, Dock.Left);

            Button Button_AddImageReceiver = new Button();
            Button_AddImageReceiver.Content = "新規";
            Button_AddImageReceiver.Click += Button_AddImageReceiver_Click;
            this.AddContent(Button_AddImageReceiver, Dock.Top);

            Button Button_RemoveImageReceiver = new Button();
            Button_RemoveImageReceiver.Content = "終了";
            Button_RemoveImageReceiver.Click += Button_RemoveImageReceiver_Click;
            this.AddContent(Button_RemoveImageReceiver, Dock.Top);

            Button Button_RemoveAllImageReceiver = new Button();
            Button_RemoveAllImageReceiver.Content = "全終了";
            Button_RemoveAllImageReceiver.Click += Button_RemoveAllImageReceiver_Click;
            this.AddContent(Button_RemoveAllImageReceiver, Dock.Top);

            StackPanel St = new StackPanel();
            St.Orientation = Orientation.Horizontal;

            TextBlock TextBlock_OpenPortNum = new TextBlock();
            TextBlock_OpenPortNum.Text = "OpenPortNum : ";
            TextBlock TextBox_OpenPortNum = new TextBlock();
            TextBox_OpenPortNum.Text = StartPortNum.ToString();
            St.Children.Add(TextBlock_OpenPortNum);
            St.Children.Add(TextBox_OpenPortNum);

            this.AddContent(St,Dock.Top);
 
            StackPanel dummy = new StackPanel();
            this.AddContent(dummy, Dock.Top);
        }

        private void Button_RemoveAllImageReceiver_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (var item in this.List_ImageReceiver)
            {
                this.m_MainWindow.RemovePanel(item);
                item.Close(this, new System.Windows.RoutedEventArgs());
            }
            this.List_ImageReceiver.Clear();
            this.UpdateListBox();
        }

        private void Button_RemoveImageReceiver_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.List_ImageReceiver.Count - 1 < this.ListBox_ReceiverHostPanel.SelectedIndex || this.ListBox_ReceiverHostPanel.SelectedIndex < 0) return;
            var item = this.List_ImageReceiver[this.ListBox_ReceiverHostPanel.SelectedIndex];
            this.List_ImageReceiver.Remove(item);

            this.m_MainWindow.RemovePanel(item);
            item.Close(this, new System.Windows.RoutedEventArgs());
            this.UpdateListBox();
        }

        private void Button_AddImageReceiver_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int i = 0;
            while (true)
            {
                if (this.List_ImageReceiver.Find(k => k.ImageReceiveID == i) == null)
                {
                    break;
                }
                ++i;
            }
            var item = new ImageReceiverPanel(i);
            item.Initialize(base.m_MainWindow);

            this.List_ImageReceiver.Add(item);
            this.m_MainWindow.AddPanel(item);
            this.m_MainWindow.DisplayPanel(item);

            this.UpdateListBox();
        }

        private void UpdateListBox()
        {
            this.ListBox_ReceiverHostPanel.Items.Clear();
            foreach (var p in this.List_ImageReceiver)
            {
                ListBoxItem item = new ListBoxItem();
                item.Content = p.TitleName + " Port:" + p.OpenPortNum.ToString();
                this.ListBox_ReceiverHostPanel.Items.Add(item);
            }
        }
    }
}
