using System;
using System.Globalization;
using System.Windows.Data;

namespace ZTask.View.Converter
{
    /// <summary>
    /// 根据 Task.IsCompleted 与 WindowInfo.IsShowCompleted 联合判断是否需要隐藏
    /// 返回true为需要隐藏
    /// </summary>
    class HideCompletedTaskConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean taskIsCompleted = (Boolean)values[0];
            Boolean isShowCompleted = (Boolean)values[1];
            return taskIsCompleted == true && isShowCompleted == false;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
