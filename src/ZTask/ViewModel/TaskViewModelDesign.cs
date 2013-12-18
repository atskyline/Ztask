using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ZTask.Model.Core.Local;

namespace ZTask.ViewModel
{
    public class TaskViewModelDesign : ViewModelBase, ITaskViewModel
    {
        public LocalTaskList TaskList { get; private set; }
        public ObservableCollection<LocalTask> Tasks { get; private set; }

        public TaskViewModelDesign()
        {
            TaskList = new LocalTaskList
            {
                Title = "测试列表名"
            };

            Tasks = new ObservableCollection<LocalTask>(new List<LocalTask>()
            {
                new LocalTask(){Title = "任务1"},
                new LocalTask(){Title = "任务2" , IsCompleted = true},
                new LocalTask(){Title = "任务3"},
                new LocalTask(){Title = "任务4", IsCompleted = true},
                new LocalTask(){Title = "任务5", IsCompleted = true},
                new LocalTask(){Title = "任务6"},
            });
        }
    }
}