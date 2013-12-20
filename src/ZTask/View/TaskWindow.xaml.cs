using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ZTask.ViewModel;

namespace ZTask.View
{
    public partial class TaskWindow : Window
    {
        public TaskWindow()
        {
            InitializeComponent();
            (this.DataContext as ITaskViewModel).View = this;
        }

        public TaskWindow(Int64 taskListId):this()
        {
            (this.DataContext as TaskViewModel).LoadData(taskListId);
        }

        private void WindowDragMove(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void OnItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
//            ((TextBox)sender).SelectAll();
//            ((TextBox)sender).IsReadOnly = false;
        }

        private void OnItemLostFocus(object sender, RoutedEventArgs e)
        {
//            ((TextBox)sender).IsReadOnly = true;
        }

        private void OnItemGotFocus(object sender, RoutedEventArgs e)
        {
            this.ListBox.SelectedItem = (sender as Grid).DataContext; 
        }

        private void OnWindowDeactivated(object sender, EventArgs eventArgs)
        {
            this.ListBox.SelectedItem = null;
        }

        //TODO CheckBox变化或TextBox完成编辑时通知VM
    }
}