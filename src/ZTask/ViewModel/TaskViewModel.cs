using System;
using System.Collections.Generic;
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
        //Command
        public RelayCommand LoadWindowInfo { get; private set; }
        public RelayCommand SaveWindowInfo { get; private set; }
        public RelayCommand AddTaskCommand { get; private set; }
        public RelayCommand<LocalTask> EditTaskCommand { get; private set; }
        public RelayCommand<LocalTask> DeleteTaskCommand { get; private set; }
        public RelayCommand CloseWindowCommand { get; private set; }
        //Config
        public Brush Background { get; private set; }
        public Brush TextForeground { get; private set; }
        public Boolean IsShowCompleted { get; set; }

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
            });
            SaveWindowInfo = new RelayCommand(() =>
            {

            });
            AddTaskCommand = new RelayCommand(() =>
            {
                
            });
            EditTaskCommand = new RelayCommand<LocalTask>((task) =>
            {

            });
            DeleteTaskCommand = new RelayCommand<LocalTask>((task) =>
            {

            });
            CloseWindowCommand = new RelayCommand(() =>
            {

            });
        }

        public void LoadData(Int64 taskListId)
        {
            TaskList = _localData.GetTaskList(taskListId);
            RaisePropertyChanged("TaskList");
            _localData.GetTasksByList(TaskList).ForEach(task => Tasks.Add(task));
        }
    }
}