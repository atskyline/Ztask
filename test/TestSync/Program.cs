using System;
using log4net;
using TestSync.Local;

namespace TestSync
{
    class Program
    {
        private static readonly ILog LOG = Util.GetLog(); 

        static void Main(string[] args)
        {
            LOG.Debug("Start Program");
            var localData = LocalData.getInstance();
            var list1 = localData.InsertTaskList(new LocalTaskList{Title = "test"});
            var task1 = localData.InsertTask(new LocalTask { Title = "task1", LocalTaskListId = list1.LocalId});
            var task2 = localData.InsertTask(new LocalTask { Title = "task2", LocalTaskListId = list1.LocalId });
            Console.WriteLine(localData.GetTaskByList(list1).Count);
            LOG.Debug("End Program");
            Console.ReadLine();
        }
    }
}
