using System.Windows;
using GalaSoft.MvvmLight.Threading;
using ZTask.Model.Core;
using ZTask.View;

namespace ZTask
{
    public partial class App : Application
    {
        private static readonly AppConfig Config = AppConfig.Load();

        static App()
        {
            DispatcherHelper.Initialize();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
//            AppConfig.Load().Proxy = "http://127.0.0.1:8087";
//            new SyncUtil().Sync();
            Config.Background = "#77000000";
            Config.TextForeground = "#FFFFFFFF";
            new TaskWindow(1).Show();
        }
    }
}
