using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ZTask.Model;
using ZTask.Model.Local;
using ZTask.View;

namespace ZTask.ViewModel
{
    public class ManagerViewModel : ViewModelBase
    {
        public virtual ObservableCollection<ManagerModel> Lists { get; set; }
        private LocalData _localData;

        public virtual RelayCommand AddCommand { get; protected set; }
        public virtual RelayCommand<ManagerModel> RenameCommand { get; protected set; }
        public virtual RelayCommand<ManagerModel> DeleteCommand { get; protected set; }
        public virtual RelayCommand<ManagerModel> UpdateCommand { get; protected set; }
        public virtual RelayCommand OnCloseCommand { get; protected set; }
        public ManagerViewModel()
        {
            Init();
        }

        protected virtual void Init()
        {
            InitData();
            InitCommand();
        }

        private void InitData()
        {
            _localData = LocalData.Instance;
            Lists = new ObservableCollection<ManagerModel>(_localData.GetAllManagerModel());
        }

        private void InitCommand()
        {
            UpdateCommand = new RelayCommand<ManagerModel>((list) =>
            {
                _localData.UpdateManagerModel(list);
            });
            AddCommand = new RelayCommand(() =>
            {
                var dialog = new InputDialog("新建列表", "新建列表的名称", "新建列表");
                if (dialog.ShowDialog() == true)
                {
                    var newList = _localData.InsertTaskList(new LocalTaskList() {Title = dialog.Input});
                    Lists.Add(new ManagerModel()
                    {
                        ListId = newList.LocalId,
                        Title = newList.Title,
                        IsHideWindow = false
                    });
                }
            });
            RenameCommand = new RelayCommand<ManagerModel>((list) =>
            {
                var dialog = new InputDialog("列表重命名", "新的列表名称", list.Title);
                if(dialog.ShowDialog() == true)
                {
                    list.Title = dialog.Input;
                    UpdateCommand.Execute(list);
                }
            });
            DeleteCommand = new RelayCommand<ManagerModel>((list) =>
            {
                var result = MessageBox.Show("您确定要删除此列表吗？", "删除列表",
                                MessageBoxButton.YesNo, MessageBoxImage.Question);
                if(result == MessageBoxResult.Yes)
                {
                    _localData.DeleteTaskListLogic(new LocalTaskList() { LocalId = list.ListId});
                    Lists.Remove(list);
                }
            });
            OnCloseCommand = new RelayCommand(() =>
            {
                //在关闭列表管理的时候，刷新一下程序
                (Application.Current as App).RefreshView();
            });
        }
    }
}