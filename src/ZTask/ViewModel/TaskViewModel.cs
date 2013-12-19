using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using Google.Apis.Tasks.v1.Data;
using ZTask.Model.Core.Local;

namespace ZTask.ViewModel
{
    public class TaskViewModel : ViewModelBase, ITaskViewModel
    {
        private static readonly AppConfig Config = AppConfig.Load();
        private LocalData _localData;

        public LocalTaskList TaskList { get; private set; }
        public ObservableCollection<LocalTask> Tasks { get; private set; }
        public Brush Background { get; private set; }
        public Brush TextForeground { get; private set; }

        public TaskViewModel()
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Config.Background));
            TextForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Config.TextForeground));
            _localData = LocalData.Instance;
            Tasks = new ObservableCollection<LocalTask>();
        }

        public void LoadData(Int64 taskListId)
        {
            TaskList = _localData.GetTaskList(taskListId);
            RaisePropertyChanged("TaskList");
            _localData.GetTasksByList(TaskList).ForEach(task => Tasks.Add(task));
        }
    }
}