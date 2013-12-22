using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ZTask.Model.Core.Local;
using ZTask.View;

namespace ZTask.ViewModel
{
    public class TaskViewModelDesign : ViewModelBase, ITaskViewModel
    {
        public TaskWindow View { set; private get; }
        public LocalTaskList TaskList { get; private set; }
        public ObservableCollection<LocalTask> Tasks { get; private set; }
        public WindowInfo WindowInfo { get; private set; }
        public RelayCommand LoadWindowInfo { get; private set; }
        public RelayCommand SaveWindowInfo { get; private set; }
        public RelayCommand AddTaskCommand { get; private set; }
        public RelayCommand<LocalTask> EditTaskCommand { get; private set; }
        public RelayCommand<LocalTask> UpdateTaskCommand { get; private set; }
        public RelayCommand<LocalTask> DeleteTaskCommand { get; private set; }
        public RelayCommand CloseWindowCommand { get; private set; }
        public Brush Background { get; private set; }
        public Brush TextForeground { get; private set; }
        public Boolean IsShowCompleted { get; set; }

        public TaskViewModelDesign()
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#77000000"));
            TextForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            TaskList = new LocalTaskList
            {
                Title = "测试列表名"
            };

            Tasks = new ObservableCollection<LocalTask>(new List<LocalTask>()
            {
                new LocalTask(){Title = "任务1"},
                new LocalTask(){Title = "任务2" , IsCompleted = true},
                new LocalTask(){Title = "任务3\n333"},
                new LocalTask(){Title = "任务4", IsCompleted = true},
                new LocalTask(){Title = "任务5", IsCompleted = true},
                new LocalTask(){Title = "任务6"},
            });

        }
    }
}