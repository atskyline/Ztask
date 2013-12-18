using System.Collections.ObjectModel;
using ZTask.Model.Core.Local;

namespace ZTask.ViewModel
{
    public interface ITaskViewModel
    {
        LocalTaskList TaskList { get; }
        ObservableCollection<LocalTask> Tasks { get; } 
    }
}