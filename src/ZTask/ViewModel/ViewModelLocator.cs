/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:ZTask.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace ZTask.ViewModel
{
    public class ViewModelLocator
    {

        public TaskViewModel TaskViewModel
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                    return new TaskViewModelDesign();
                }
                return new TaskViewModel();
            }
        }

        public ManagerViewModel ManagerViewModel
        {
            get
            {
                if(ViewModelBase.IsInDesignModeStatic)
                {
                    return new ManagerViewModelDesign();
                }
                return new ManagerViewModel();
            }
        }

    }
}