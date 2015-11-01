using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LabLife.Contorols
{
    /// <summary>
    /// Interaction logic for SliderAndTextControl.xaml
    /// </summary>
    public partial class SliderAndTextControl : UserControl
    {
        public double Value
        {
            set
            {
                this.Slider_Main.Value = value;
            }

            get
            {
                return this.Slider_Main.Value;
            }
        }
        public SliderAndTextControl()
        {
            InitializeComponent();
        }
    }
}
