using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using GalaSoft.MvvmLight.Command;
using ZTask.Model.Core.Local;
using ZTask.View;

namespace ZTask.ViewModel
{
    public interface ITaskViewModel
    {
        TaskWindow View { set; }
        //Data
        LocalTaskList TaskList { get; }
        ObservableCollection<LocalTask> Tasks { get; }
        WindowInfo WindowInfo { get; }
        //Command
        RelayCommand LoadWindowInfo { get; }
        RelayCommand SaveWindowInfo { get; }
        RelayCommand AddTaskCommand { get; }
        RelayCommand<LocalTask> EditTaskCommand { get; }
        RelayCommand<LocalTask> UpdateTaskCommand { get; }
        RelayCommand<LocalTask> DeleteTaskCommand { get; }
        RelayCommand CloseWindowCommand { get; }
        //Config
        Brush Background { get; }
        Brush TextForeground { get; }
        Boolean IsShowCompleted { get; set; }
    }
}