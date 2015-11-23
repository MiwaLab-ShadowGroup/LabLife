using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LabLife
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string message = string.Format(
                "大変申し訳けありません。システムエラーが発生しました。\n ({0} {1})",
                e.Exception.GetType(), e.Exception.Message);
            MessageBox.Show(message);
            e.Handled = true;
        }
    }
}
