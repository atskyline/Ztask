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
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<ITaskViewModel, TaskViewModelDesign>();
            }
            else
            {
                SimpleIoc.Default.Register<ITaskViewModel, TaskViewModel>();
            }
        }

        public ITaskViewModel TaskViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ITaskViewModel>();
            }
        }

    }
}