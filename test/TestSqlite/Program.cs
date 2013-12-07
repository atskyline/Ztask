using System;
using System.Data.SQLite;
using System.Xml;

namespace TestSqlite
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = LocalData.getInstance();
            var list = db.InsertTaskList(new TaskList(){Title = "new"});
            Console.WriteLine(list.Id);
            Console.ReadLine();
        }
    }
}
