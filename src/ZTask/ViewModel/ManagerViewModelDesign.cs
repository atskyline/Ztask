using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using ZTask.Model;

namespace ZTask.ViewModel
{
    public class ManagerViewModelDesign : ManagerViewModel
    {
        public ManagerViewModelDesign()
        {
            Lists = new ObservableCollection<ManagerModel>();
            Lists.Add(new ManagerModel(){Title = "List1",IsHideWindow = false});
            Lists.Add(new ManagerModel() { Title = "List2", IsHideWindow = true });
            Lists.Add(new ManagerModel() { Title = "List3", IsHideWindow = false });
            Lists.Add(new ManagerModel() { Title = "List4", IsHideWindow = false });
        }

        //重载Init,Design模式下不需要初始化资源
        protected override void Init()
        {
        }
    }
}