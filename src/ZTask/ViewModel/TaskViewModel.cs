using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Google.Apis.Tasks.v1.Data;
using ZTask.Model.Core.Local;
using ZTask.View;

namespace ZTask.ViewModel
{
    public class TaskViewModel : ViewModelBase, ITaskViewModel
    {
        private static readonly AppConfig Config = AppConfig.Load();
        private LocalData _localData;

        //Data
        public TaskWindow View { set; private get; }
        public LocalTaskList TaskList { get; private set; }
        public ObservableCollection<LocalTask> Tasks { get; private set; }
        public WindowInfo WindowInfo { get; private set; }
        //Command
        public RelayCommand LoadWindowInfo { get; private set; }
        public RelayCommand SaveWindowInfo { get; private set; }
        public RelayCommand AddTaskCommand { get; private set; }
        public RelayCommand<LocalTask> EditTaskCommand { get; private set; }
        public RelayCommand<LocalTask> UpdateTaskCommand { get; private set; }
        public RelayCommand<LocalTask> DeleteTaskCommand { get; private set; }
        public RelayCommand CloseWindowCommand { get; private set; }
        //Config
        public Brush Background { get; private set; }
        public Brush TextForeground { get; private set; }

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
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Config.Background));
            TextForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Config.TextForeground));
            _localData = LocalData.Instance;
            Tasks = new ObservableCollection<LocalTask>();
            InitCommand();
        }

        private void InitCommand()
        {
            LoadWindowInfo = new RelayCommand(() =>
            {
                View.Left = WindowInfo.Left;
                View.Top = WindowInfo.Top;
                View.Height = WindowInfo.Height;
                View.Width = WindowInfo.Width;
            });
            SaveWindowInfo = new RelayCommand(() =>
            {
                WindowInfo.Left = View.Left;
                WindowInfo.Top = View.Top;
                WindowInfo.Height = View.Height;
                WindowInfo.Width = View.Width;
                _localData.UpdateWindowInfo(WindowInfo);
            });
            AddTaskCommand = new RelayCommand(() =>
            {
                var newTask = _localData.InsertTask(new LocalTask());
                Tasks.Add(newTask);
            });
            EditTaskCommand = new RelayCommand<LocalTask>((task) =>
            {
                //TODO 显示TaskDetail界面
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

        public void LoadData(Int64 taskListId)
        {
            TaskList = _localData.GetTaskList(taskListId);
            RaisePropertyChanged("TaskList");
            WindowInfo = _localData.GetWindowInfoByTaskList(TaskList);
            _localData.GetTasksByList(TaskList,false).ForEach(task => Tasks.Add(task));
        }
    }
}