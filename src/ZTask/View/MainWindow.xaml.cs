using System;
using System.Windows;
using ZTask.ViewModel;

namespace ZTask.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //TODO 加载 Top Left Height Width 
        }

        private void OnClosed(object sender, EventArgs e)
        {
            //TODO 保存 Top Left Height Width 
        }
    }
}