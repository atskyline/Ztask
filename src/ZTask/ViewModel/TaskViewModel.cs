using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ZTask.Model.Core.Local;

namespace ZTask.ViewModel
{
    public class TaskViewModel : ViewModelBase, ITaskViewModel
    {
        public LocalTaskList TaskList { get; private set; }
        public ObservableCollection<LocalTask> Tasks { get; private set; }

        public TaskViewModel()
        {
        }
    }
}