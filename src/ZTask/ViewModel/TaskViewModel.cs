using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Google.Apis.Tasks.v1.Data;
using ZTask.Model.Local;
using ZTask.View;

namespace ZTask.ViewModel
{
    public class TaskViewModel : ViewModelBase
    {
        protected static readonly AppConfig Config = AppConfig.Load();
        private LocalData _localData;

        //Data
        public virtual TaskWindow View { set; protected get; }
        public virtual LocalTaskList TaskList { get; protected set; }
        public virtual ObservableCollection<LocalTask> Tasks { get; protected set; }
        public virtual WindowInfo WindowInfo { get; set; }
        //Command
        public virtual RelayCommand LoadLocationCommand { get; protected set; }
        public virtual RelayCommand SaveLocationCommand { get; protected set; }
        public virtual RelayCommand AddTaskCommand { get; protected set; }
        public virtual RelayCommand<LocalTask> UpdateTaskCommand { get; protected set; }
        public virtual RelayCommand<LocalTask> DeleteTaskCommand { get; protected set; }
        public virtual RelayCommand CloseWindowCommand { get; protected set; }
        //Config
        public virtual Brush Background { get; protected set; }
        public virtual Brush TextForeground { get; protected set; }

        public Boolean IsShowCompleted
        {
            get
            {
                if (WindowInfo == null) return false;
                return WindowInfo.IsShowCompleted;
            }
            set
            {
                if (WindowInfo == null) return;
                WindowInfo.IsShowCompleted = value;
                RaisePropertyChanged("IsShowCompleted");
                _localData.UpdateWindowInfo(WindowInfo);
            }
        }

        public TaskViewModel()
        {
            Init();
        }

        protected virtual void Init()
        {
            _localData = LocalData.Instance;
            Tasks = new ObservableCollection<LocalTask>();
            //读取配置
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Config.Background));
            TextForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Config.TextForeground));
            
            InitCommand();
        }

        private void InitCommand()
        {
            LoadLocationCommand = new RelayCommand(() =>
            {
                View.Left = WindowInfo.Left;
                View.Top = WindowInfo.Top;
                View.Height = WindowInfo.Height;
                View.Width = WindowInfo.Width;
            });
            SaveLocationCommand = new RelayCommand(() =>
            {
                WindowInfo.Left = View.Left;
                WindowInfo.Top = View.Top;
                WindowInfo.Height = View.Height;
                WindowInfo.Width = View.Width;
                _localData.UpdateWindowInfo(WindowInfo);
            });
            AddTaskCommand = new RelayCommand(() =>
            {
                var newTask = _localData.InsertTask(new LocalTask() { LocalTaskListId = TaskList.LocalId});
                Tasks.Add(newTask);
            });
            UpdateTaskCommand = new RelayCommand<LocalTask>((task) =>
            {
                _localData.UpdateTask(task);
            });
            DeleteTaskCommand = new RelayCommand<LocalTask>((task) =>
            {
                _localData.DeleteTaskLogic(task);
                Tasks.Remove(task);
            });
            CloseWindowCommand = new RelayCommand(() =>
            {
                WindowInfo.IsHideWindow = true;
                _localData.UpdateWindowInfo(WindowInfo);
                View.Close();
            });
        }

        public void LoadData(WindowInfo winInfo)
        {
            WindowInfo = winInfo;
            LoadData();
        }

        /// <summary>
        /// 根据WindowInfo加载数据
        /// </summary>
        public void LoadData()
        {
            TaskList = _localData.GetTaskList(WindowInfo.TaskListId);
            RaisePropertyChanged("TaskList");
            _localData.GetTasksByList(TaskList, false).ForEach(task => Tasks.Add(task));
        }
    }
}