using System;
using System.Collections.Generic;
using System.Data.SQLite;
using log4net;

namespace TestSync.Local
{
    class LocalData
    {
        private static readonly ILog LOG = Util.GetLog(); 

        private const String CONNECT_STRING = @"Data Source=./LocalDB.db";

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

        private static String CastToString(Object obj)
        {
            if (obj == DBNull.Value || obj == null)
                return null;

            return obj.ToString();
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
                    command.CommandText =
@"CREATE TABLE IF NOT EXISTS TaskList(
    LocalId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    LocalModify BOOLEAN,
    LocalDelete BOOLEAN,
    Id TEXT,
    Title TEXT
);
CREATE TABLE IF NOT EXISTS Task(
    LocalId INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    LocalTaskListId INTEGER,
    LocalModify BOOLEAN,
    LocalDelete BOOLEAN,
    Id TEXT,
    Title TEXT,
    ETag TEXT,
    Due TEXT,
    Notes TEXT,
    Parent TEXT,
    Position TEXT,
    Status TEXT
);
";
                    command.ExecuteNonQuery();
                }
            }
        }

        private LocalTaskList buildTaskList(SQLiteDataReader reader)
        {
            return new LocalTaskList
            {
                LocalId = (Int64)reader["LocalId"],
                LocalModify = (Boolean)reader["LocalModify"],
                LocalDelete = (Boolean)reader["LocalDelete"],
                Id = CastToString(reader["Id"]),
                Title = CastToString(reader["Title"])
            };
        }

        public List<LocalTaskList> GetAllTaskList()
        {
            var result = new List<LocalTaskList>();
            using (var connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT * FROM TaskList";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(buildTaskList(reader));
                        }
                    }
                }
            }
            return result;
        }

        public LocalTaskList GetTaskListByLocalId(Int64 localId)
        {
            LocalTaskList result = null;
            using (var connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT * FROM TaskList WHERE LocalId = @LocalId";
                    command.Parameters.Add(new SQLiteParameter("@LocalId", localId));
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = buildTaskList(reader);
                        }
                    }
                }
            }
            return result;
        }

        public LocalTaskList GetTaskListById(String Id)
        {
            LocalTaskList result = null;
            using (var connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT * FROM TaskList WHERE Id = @Id";
                    command.Parameters.Add(new SQLiteParameter("@Id", Id));
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = buildTaskList(reader);
                        }
                    }
                }
            }
            return result;
        }

        public LocalTaskList InsertTaskList(LocalTaskList taskList)
        {
            LocalTaskList result = null;
            using (var connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"INSERT INTO TaskList VALUES (null,@LocalModify,@LocalDelete,@Id,@Title);SELECT last_insert_rowid();";
                    command.Parameters.Add(new SQLiteParameter("@LocalModify", taskList.LocalModify));
                    command.Parameters.Add(new SQLiteParameter("@LocalDelete", taskList.LocalDelete));
                    command.Parameters.Add(new SQLiteParameter("@Id", taskList.Id));
                    command.Parameters.Add(new SQLiteParameter("@Title", taskList.Title));
                    var localId = (long)command.ExecuteScalar();
                    result = taskList.Clone();
                    result.LocalId = localId;
                }
            }
            LOG.Info("Local Insert TaskList " + result);
            return result;
        }

        public void UpdateTaskList(LocalTaskList taskList)
        {
            using (var connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"UPDATE TaskList SET LocalModify = @LocalModify,LocalDelete = @LocalDelete,Id = @Id,Title = @Title WHERE LocalId = @LocalId;";
                    command.Parameters.Add(new SQLiteParameter("@LocalId", taskList.LocalId));
                    command.Parameters.Add(new SQLiteParameter("@LocalModify", taskList.LocalModify));
                    command.Parameters.Add(new SQLiteParameter("@LocalDelete", taskList.LocalDelete));
                    command.Parameters.Add(new SQLiteParameter("@Id", taskList.Id));
                    command.Parameters.Add(new SQLiteParameter("@Title", taskList.Title));
                    command.ExecuteNonQuery();
                }
            }
            LOG.Info("Local Update TaskList " + taskList);
        }

        public void DeleteTaskList(LocalTaskList taskList)
        {
            using (var connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"DELETE FROM TaskList WHERE LocalId = @LocalId;";
                    command.Parameters.Add(new SQLiteParameter("@LocalId", taskList.LocalId));
                    command.ExecuteNonQuery();
                }
            }
            LOG.Info("Local Delete TaskList " + taskList);
        }

        private LocalTask buildTask(SQLiteDataReader reader)
        {
            return new LocalTask
            {
                LocalId = (Int64)reader["LocalId"],
                LocalModify = (Boolean)reader["LocalModify"],
                LocalDelete = (Boolean)reader["LocalDelete"],
                Id = CastToString(reader["Id"]),
                Title = CastToString(reader["Title"]),
                ETag = CastToString(reader["ETag"]),
                Due = CastToString(reader["Due"]),
                Notes = CastToString(reader["Notes"]),
                Parent = CastToString(reader["Parent"]),
                Position = CastToString(reader["Position"]),
                Status = CastToString(reader["Status"]),
            };
        }

        public LocalTask GetTaskById(String Id)
        {
            LocalTask result = null;
            using (var connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT * FROM Task WHERE Id = @Id";
                    command.Parameters.Add(new SQLiteParameter("@Id", Id));
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = buildTask(reader);
                        }
                    }
                }
            }
            return result;
        }

        public List<LocalTask> GetTaskByList(LocalTaskList list)
        {
            var result = new List<LocalTask>();
            using (var connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT * FROM Task WHERE LocalTaskListId = @LocalTaskListId";
                    command.Parameters.Add(new SQLiteParameter("@LocalTaskListId", list.LocalId));
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(buildTask(reader));
                        }
                    }
                }
            }
            return result;
        }

        public LocalTask InsertTask(LocalTask task)
        {
            LocalTask result = null;
            using (var connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText =
@"INSERT INTO Task VALUES 
(null,@LocalTaskListId,@LocalModify,@LocalDelete,@Id,@Title,@ETag,@Due,@Notes,@Parent,@Position,@Status);
SELECT last_insert_rowid();";
                    command.Parameters.Add(new SQLiteParameter("@LocalTaskListId", task.LocalTaskListId));
                    command.Parameters.Add(new SQLiteParameter("@LocalModify", task.LocalModify));
                    command.Parameters.Add(new SQLiteParameter("@LocalDelete", task.LocalDelete));
                    command.Parameters.Add(new SQLiteParameter("@Id", task.Id));
                    command.Parameters.Add(new SQLiteParameter("@Title", task.Title));
                    command.Parameters.Add(new SQLiteParameter("@Title", task.Title));
                    command.Parameters.Add(new SQLiteParameter("@ETag", task.ETag));
                    command.Parameters.Add(new SQLiteParameter("@Due", task.Due));
                    command.Parameters.Add(new SQLiteParameter("@Notes", task.Notes));
                    command.Parameters.Add(new SQLiteParameter("@Parent", task.Parent));
                    command.Parameters.Add(new SQLiteParameter("@Position", task.Position));
                    command.Parameters.Add(new SQLiteParameter("@Status", task.Status));
                    var localId = (long)command.ExecuteScalar();
                    result = task.Clone();
                    result.LocalId = localId;
                }
            }
            LOG.Info("Local Insert Task " + result);
            return result;
        }

        public void UpdateTask(LocalTask task)
        {
            using (var connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText =
@"UPDATE Task
SET LocalTaskListId = @LocalTaskListId ,LocalModify = @LocalModify,LocalDelete = @LocalDelete,Id = @Id,Title = @Title,
ETag = @ETag, Due = @Due, Notes = @Notes, Parent = @Parent, Position = @Position, Status = @Status
WHERE LocalId = @LocalId;";
                    command.Parameters.Add(new SQLiteParameter("@LocalId", task.LocalId));
                    command.Parameters.Add(new SQLiteParameter("@LocalTaskListId", task.LocalTaskListId));
                    command.Parameters.Add(new SQLiteParameter("@LocalModify", task.LocalModify));
                    command.Parameters.Add(new SQLiteParameter("@LocalDelete", task.LocalDelete));
                    command.Parameters.Add(new SQLiteParameter("@Id", task.Id));
                    command.Parameters.Add(new SQLiteParameter("@Title", task.Title));
                    command.Parameters.Add(new SQLiteParameter("@Title", task.Title));
                    command.Parameters.Add(new SQLiteParameter("@ETag", task.ETag));
                    command.Parameters.Add(new SQLiteParameter("@Due", task.Due));
                    command.Parameters.Add(new SQLiteParameter("@Notes", task.Notes));
                    command.Parameters.Add(new SQLiteParameter("@Parent", task.Parent));
                    command.Parameters.Add(new SQLiteParameter("@Position", task.Position));
                    command.Parameters.Add(new SQLiteParameter("@Status", task.Status));
                    command.ExecuteNonQuery();
                }
            }
            LOG.Info("Local Update Task " + task);
        }

        public void DeleteTask(LocalTask task)
        {
            using (var connection = new SQLiteConnection(CONNECT_STRING))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"DELETE FROM Task WHERE LocalId = @LocalId;";
                    command.Parameters.Add(new SQLiteParameter("@LocalId", task.LocalId));
                    command.ExecuteNonQuery();
                }
            }
            LOG.Info("Local Delete Task " + task);
        }

    }
}
