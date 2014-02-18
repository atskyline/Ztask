using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ZTask.Model.Local;
using ZTask.View;

namespace ZTask.ViewModel
{
    public class TaskViewModelDesign : TaskViewModel
    {

        public TaskViewModelDesign()
        {
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#77000000"));
            TextForeground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFFFF"));
            TaskList = new LocalTaskList
            {
                Title = "测试列表名"
            };

            Tasks = new ObservableCollection<LocalTask>(new List<LocalTask>()
            {
                new LocalTask(){Title = "任务1"},
                new LocalTask(){Title = "任务2" , IsCompleted = true},
                new LocalTask(){Title = "任务3\n333"},
                new LocalTask(){Title = "任务4", IsCompleted = true},
                new LocalTask(){Title = "任务5", IsCompleted = true},
                new LocalTask(){Title = "任务6"},
            });
        }

        //重载Init,Design模式下不需要初始化资源
        protected override void Init()
        {
        }
    }
}