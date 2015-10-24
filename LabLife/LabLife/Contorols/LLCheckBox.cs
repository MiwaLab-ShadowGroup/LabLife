using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LabLife.Contorols
{
    public class LLCheckBox : Button
    {
        public bool IsChecked;
        public LLCheckBox(bool Default = false)
        {
            this.Click += LLCheckBox_Click;
            this.IsChecked = Default;
            this.UpdateStyle();
        }
        public void UpdateStyle()
        {
            if (this.IsChecked)
            {
                this.Style = (Style)App.Current.Resources["WindowButtonCheckOnStyle"];
            }
            else
            {
                this.Style = (Style)App.Current.Resources["WindowButtonCheckOffStyle"];
            }

        }
        private void LLCheckBox_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            IsChecked = !IsChecked;
            this.UpdateStyle();
        }
    }
}
