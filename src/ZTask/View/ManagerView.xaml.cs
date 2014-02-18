using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZTask.ViewModel;

namespace ZTask.View
{
    public partial class ManagerView : Window
    {
        private readonly ManagerViewModel _viewModel;
        public ManagerView()
        {
            InitializeComponent();
            _viewModel = this.DataContext as ManagerViewModel;
        }

        private void OnCheckChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateCommand.Execute(((CheckBox)e.OriginalSource).DataContext);
        }

        private void OnItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _viewModel.RenameCommand.Execute(this.ListView.SelectedItem);
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            _viewModel.OnCloseCommand.Execute(null);
        }
    }
}