using System;
using System.Windows;

namespace ZTask.View
{
    //提供标题，内容，输入框，确定按钮，取消按钮的对话框
    public partial class InputDialog : Window
    {
        public String Input { get; set; }

        public InputDialog(String title,String content,String defaultInput)
        {
            InitializeComponent();
            this.Title = title;
            this.Label.Content = content;
            this.TextBox.Text = defaultInput;
        }

        private void OnYesButtonClick(object sender, RoutedEventArgs e)
        {
            this.Input = this.TextBox.Text;
            this.DialogResult = true;
            this.Close();
        }

        private void OnNoButtonClick(object sender, RoutedEventArgs e)
        {
            this.Input = this.TextBox.Text;
            this.DialogResult = false;
            this.Close();
        }

        private void InputDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.TextBox.SelectAll();
            this.TextBox.Focus();
        }
    }
}