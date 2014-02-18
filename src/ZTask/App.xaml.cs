using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Threading;
using log4net;
using ZTask.Model;
using ZTask.Model.Local;
using ZTask.View;
using Application = System.Windows.Application;

namespace ZTask
{
    public partial class App : Application
    {
        private static readonly AppConfig Config = AppConfig.Load();
        private static readonly ILog Log = Util.Log;

        private LocalData _localData;
        private List<TaskWindow> _taskWindows;
        private NotifyIcon _trayIcon;

        static App()
        {
            DispatcherHelper.Initialize();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            _localData = LocalData.Instance;
            _taskWindows = new List<TaskWindow>();
            this.MainWindow = new Window();
            InitConfig();
            InitTrayIcon();
            InitTimerTask();
            RefreshView();
        }

        private static void InitConfig()
        {
            if(String.IsNullOrEmpty(Config.Background)) Config.Background = "#77000000";
            if(String.IsNullOrEmpty(Config.TextForeground)) Config.TextForeground = "#FFFFFFFF";
            if(String.IsNullOrEmpty(Config.Proxy)) Config.Proxy = "";
            if(Config.AutoSync == null) Config.AutoSync = false;
            if(Config.AutoSyncInterval == null) Config.AutoSyncInterval = 120;
            Config.Save();
        }

        private void InitTrayIcon()
        {
            _trayIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon("ico.ico"),
                Text = "ZTask"
            };
            _trayIcon.Visible = true;

            //设置NotifyIcon的右键弹出菜单
            var menu = new ContextMenu();
            //立即同步
            var syncItem = new MenuItem();
            syncItem.Text = "立即同步";
            syncItem.Click += (o, e) =>
            {
                syncItem.Text = "同步中...";
                syncItem.Enabled = false;
                var action = new Action(()=>new SyncUtil().Sync());
                action.BeginInvoke((ar) =>
                {
                    try
                    {
                        action.EndInvoke(ar);
                        _trayIcon.BalloonTipTitle = "同步成功";
                        _trayIcon.BalloonTipText = "ZTask与GTask同步成功！";
                        _trayIcon.ShowBalloonTip(1000);
                    }
                    catch (HttpRequestException ex)
                    {
                        Log.Info("同步遇到网络异常");
                        _trayIcon.BalloonTipTitle = "同步失败";
                        _trayIcon.BalloonTipText = "请检查您的网络情况或代理设置";
                        _trayIcon.ShowBalloonTip(1000);
                    }
                    catch (Exception ex)
                    {
                        Log.Error("同步遇到未知异常", ex);
                        _trayIcon.BalloonTipTitle = "同步失败";
                        _trayIcon.BalloonTipText = "同步遇到未知异常，请查看log.txt";
                        _trayIcon.ShowBalloonTip(1000);
                    }
                    finally
                    {
                        syncItem.Text = "立即同步...";
                        syncItem.Enabled = true;
                        this.Dispatcher.BeginInvoke((ThreadStart) RefreshView);
                    }
                }, null);
            };
            //显示所有列表
            menu.MenuItems.Add(syncItem);
            var showAllItem = new MenuItem();
            showAllItem.Text = "显示所有列表";
            showAllItem.Click += (o, e) =>
            {
                _localData.GetAllWindowInfo(true).ForEach(w =>
                {
                    if (w.IsHideWindow == true)
                    {
                        w.IsHideWindow = false;
                        _localData.UpdateWindowInfo(w);
                    }
                });
                RefreshView();
            };
            menu.MenuItems.Add(showAllItem);
            //列表管理
            var managerItem = new MenuItem();
            managerItem.Text = "列表管理";
            managerItem.Click += (o, e) =>
            {
                new ManagerView().Show();
            };
            menu.MenuItems.Add(managerItem);
            //打开配置文件
            var settingItem = new MenuItem();
            settingItem.Text = "打开配置文件";
            settingItem.Click += (o, e) =>
            {
                System.Diagnostics.Process.Start(AppConfig.ConfigFilePath);
            };
            menu.MenuItems.Add(settingItem);
            //退出程序
            var closeItem = new MenuItem();
            closeItem.Text = "退出程序";
            closeItem.Click += (o,e) => Shutdown();
            menu.MenuItems.Add(closeItem);

            _trayIcon.ContextMenu = menu;
        }

        private void InitTimerTask()
        {
            if(Config.AutoSync == false || Config.AutoSyncInterval.GetValueOrDefault() <= 0)
                return;
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(Config.AutoSyncInterval.GetValueOrDefault());
            timer.Tick += (o, e) =>
            {
                var action = new Action(() => new SyncUtil().Sync());
                action.BeginInvoke((ar) =>
                {
                    try
                    {
                        action.EndInvoke(ar);
                        Log.Info("同步完成");
                    }
                    catch(HttpRequestException ex)
                    {
                        Log.Info("同步遇到网络异常");
                    }
                    catch(Exception ex)
                    {
                        Log.Error("同步遇到未知异常", ex);
                    }
                    finally
                    {
                        this.Dispatcher.BeginInvoke((ThreadStart)RefreshView);
                    }
                }, null);
            };
            timer.Start();
        }

        public void RefreshView()
        {
            foreach(var win in _taskWindows)
            {
                win.Close();
            }
            _taskWindows.Clear();
            var winInfos = _localData.GetAllWindowInfo(false);
            foreach(var winInfo in winInfos)
            {
                var win = new TaskWindow(winInfo);
                _taskWindows.Add(win);
                win.Show();
            }
        }

        protected void OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            if (_trayIcon != null)
            {
                _trayIcon.Dispose();
            }
            if(this.MainWindow != null)
            {
                this.MainWindow.Close();
            }
        }
    }
}
