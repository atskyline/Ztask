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
        }

        public TaskWindow(Int64 taskListId):this()
        {
            (this.DataContext as TaskViewModel).LoadData(taskListId);
        }

        private void WindowDragMove(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //TODO 加载 Top Left Height Width 
        }

        private void OnClosed(object sender, EventArgs e)
        {
            //TODO 保存 Top Left Height Width 
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
    }
}