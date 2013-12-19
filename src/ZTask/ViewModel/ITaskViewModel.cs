using System.Collections.ObjectModel;
using System.Windows.Media;
using ZTask.Model.Core.Local;

namespace ZTask.ViewModel
{
    public interface ITaskViewModel
    {
        LocalTaskList TaskList { get; }
        ObservableCollection<LocalTask> Tasks { get; }

        Brush Background { get; }
        Brush TextForeground { get; }
    }
}