using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using TestDataTemplate.Model;

namespace TestDataTemplate.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        public ObservableCollection<DataItem> DataList { get; private set; } 

        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            DataList = new ObservableCollection<DataItem>(dataService.List());
        }

    }
}