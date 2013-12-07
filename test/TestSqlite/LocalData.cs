using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSqlite
{
    class LocalData
    {
        private const String CONNECT_STRING = @"Data Source=./test.db";

        private static LocalData instance;
        public static LocalData getInstance()
        {
            if (instance == null)
            {
                instance = new LocalData();
                instance.init();
            }
            return instance;
        }

        private LocalData()
        {
            
        }

        /// <summary>
        /// 初始化数据库，包括新建表等操作
        /// </summary>
        private void init()
        {
            using (SQLiteConnection connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS TaskList(
                                              db_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                              title TEXT);
                                            CREATE TABLE IF NOT EXISTS Task(
                                              db_id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                                              gtask_id TEXT,
                                              title TEXT,
                                              completed TEXT,
                                              list_id INTEGER);
                                            ";
                    command.ExecuteNonQuery();
                }
            }
        }

        public TaskList InsertTaskList(TaskList taskList)
        {
            TaskList result = null;
            using (SQLiteConnection connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (SQLiteCommand command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"INSERT INTO TaskList ( title ) VALUES ( @title );SELECT last_insert_rowid();";
                    command.Parameters.Add(new SQLiteParameter("@title",taskList.Title));
                    var id = (long)command.ExecuteScalar();
                    result = taskList.Clone();
                    result.Id = id;
                }
            }
            return result;
        }
    }
}
