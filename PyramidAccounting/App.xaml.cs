using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Text;
using System.Windows.Threading;

namespace PyramidAccounting
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Get Reference to the current Process
            Process thisProc = Process.GetCurrentProcess();
            // Check how many total processes have the same name as the current one
            if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
            {
                // If ther is more than one, than it is already running.
                MessageBox.Show("金字塔财务管理工具正在运行。");
                Application.Current.Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("应用程序出现了未捕获的异常，{0}/n", e.Exception.Message);
            if (e.Exception.InnerException != null)
            {
                stringBuilder.AppendFormat("/n {0}", e.Exception.InnerException.Message);
            }
            stringBuilder.AppendFormat("/n {0}", e.Exception.StackTrace);
            MessageBox.Show(stringBuilder.ToString());
            PA.Helper.DataBase.Log.Write(stringBuilder.ToString());
            e.Handled = true;
        }  
    }
}
