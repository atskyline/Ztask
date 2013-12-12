using System;
using log4net;
using TestSync.Local;

namespace TestSync
{
    class Program
    {
        static void Main(string[] args)
        {
//            var localData = LocalData.getInstance();
//            var list = localData.GetAllTaskList()[6];
//            localData.InsertTask(new LocalTask() {Title = "new1",LocalTaskListId = list.LocalId});
            var syncUtil = new SyncUtil();
            try
            {
                syncUtil.Sync();
            }
            catch (System.Net.Http.HttpRequestException e)
            {
                Console.WriteLine("网络异常");
            }
            Console.ReadLine();
        }
    }
}
