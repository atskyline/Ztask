using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using ZTask.Model.Local;
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

        

        private readonly TaskViewModel _viewModel;
        public TaskWindow()
        {
            InitializeComponent();
            _viewModel = this.DataContext as TaskViewModel;
            _viewModel.View = this;
        }

        /// <summary>
        /// 在窗口初始化的时候不会触发ToggleButton的ToggleButton.Template
        /// 所以在这里手动触发一次
        /// </summary>
        public TaskWindow(WindowInfo winInfo):this()
        {
            _viewModel.LoadData(winInfo);
            this.IsShowCompletedButton.IsChecked = _viewModel.IsShowCompleted;
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel.LoadLocationCommand.Execute(null);
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            _viewModel.SaveLocationCommand.Execute(null);
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

        private void OnItemGotFocus(object sender, RoutedEventArgs e)
        {
            this.ListBox.SelectedItem = (sender as Grid).DataContext; 
        }

        private TextBox _lastChangedTextBox;
        //TextBox完成编辑时调用UpdateTaskCommand
        //OnItemTextBoxChanged时认为开始修改
        //OnItemTextBoxLostFocus或者OnWindowDeactivated时认为修改完成
        private void OnItemTextBoxChanged(object sender, TextChangedEventArgs e)
        {
            //使用Tag标记文本已经有变化
            (sender as TextBox).Tag = true;
            _lastChangedTextBox = sender as TextBox;
        }

        private void OnItemTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            var flag = (sender as TextBox).Tag as Boolean?;
            if (flag == true)
            {
                _viewModel.UpdateTaskCommand.Execute(((TextBox)e.OriginalSource).DataContext);
                (sender as TextBox).Tag = false;
            }
        }

        private void OnWindowDeactivated(object sender, EventArgs eventArgs)
        {
            if(_lastChangedTextBox != null)
            {
                var flag = _lastChangedTextBox.Tag as Boolean?;
                if(flag == true)
                {
                    _viewModel.UpdateTaskCommand.Execute(_lastChangedTextBox.DataContext);
                    _lastChangedTextBox.Tag = false;
                }
            }
            this.ListBox.SelectedItem = null;
        }

        //CheckBox变化时调用UpdateTaskCommand
        private void OnItemCheckChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.UpdateTaskCommand.Execute(((CheckBox)e.OriginalSource).DataContext);
        }
    }
}