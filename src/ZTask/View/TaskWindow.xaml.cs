using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ZTask.ViewModel;

namespace ZTask.View
{
    public partial class TaskWindow : Window
    {
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;
        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        private ITaskViewModel ViewModel;
        public TaskWindow()
        {
            InitializeComponent();
            ViewModel = this.DataContext as ITaskViewModel;
            ViewModel.View = this;
        }

        public TaskWindow(Int64 taskListId):this()
        {
            (ViewModel as TaskViewModel).LoadData(taskListId);
        }

        private void WindowDragMove(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        //保持窗体在底层
        private void OnWindowActivited(object sender, EventArgs e)
        {
            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE
                                                       | SWP_NOMOVE
                                                       | SWP_NOACTIVATE);
        }

        private void OnWindowDeactivated(object sender, EventArgs eventArgs)
        {
            this.ListBox.SelectedItem = null;
        }

        private void OnItemGotFocus(object sender, RoutedEventArgs e)
        {
            this.ListBox.SelectedItem = (sender as Grid).DataContext; 
        }

        //TextBox完成编辑时调用UpdateTaskCommand
        private void OnItemTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            //使用Tag标记文本已经有变化
            (sender as TextBox).Tag = true;
        }

        private void OnItemTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var flag = (sender as TextBox).Tag as Boolean?;
            if (flag == true)
            {
                ViewModel.UpdateTaskCommand.Execute(this.ListBox.SelectedItem);
                (sender as TextBox).Tag = false;
            }
        }

        //CheckBox变化时调用UpdateTaskCommand
        private void OnItemChecked(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdateTaskCommand.Execute(this.ListBox.SelectedItem);
        }

    }
}