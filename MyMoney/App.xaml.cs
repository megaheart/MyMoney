using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMoney
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppSetting appSetting = new AppSetting();
            appSetting.Setup();
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            MainWindow window = new MainWindow();
            window.Show();
        }
        public BackgroundWorker RunAsync(DoWorkEventHandler action, ProgressChangedEventHandler progressReport, Action callback)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += action;
            worker.ProgressChanged += progressReport;
            worker.RunWorkerCompleted += (sender, e) => { callback(); };
            worker.RunWorkerAsync();
            return worker;
        }
    }
}
