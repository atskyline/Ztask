﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using log4net;

namespace ZTask.Model.Local
{
    public class LocalData
    {
        private static readonly ILog Log = Util.Log; 

        private const String ConnectString = @"Data Source=./LocalDB.db";

        private static LocalData _instance;
        public static LocalData Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LocalData();
                    _instance.Init();
                }
                return _instance;
            }
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

        private static DateTime? CastToDateTime(Object obj)
        {
            if (obj == DBNull.Value || obj == null)
                return null;
            return DateTime.Parse(obj.ToString());
        }

        /// <summary>
        /// 初始化数据库，包括新建表等操作
        /// </summary>
        private void Init()
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectString))
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
CREATE TABLE IF NOT EXISTS WindowInfo(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    TaskListId INTEGER,
    Left REAL,
    Top REAL,
    Height REAL,
    Width REAL,
    IsHideWindow BOOLEAN,
    IsShowCompleted BOOLEAN
);
";
                    command.ExecuteNonQuery();
                }
            }
        }

        private static LocalTaskList BuildTaskList(SQLiteDataReader reader)
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

        public LocalTaskList GetTaskList(Int64 listId)
        {
            LocalTaskList result = null;
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT * FROM TaskList WHERE LocalId = @LocalId;";
                    command.Parameters.Add(new SQLiteParameter("@LocalId", listId));
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = BuildTaskList(reader);
                        }
                    }
                }
            }
            return result;
        }

        public List<LocalTaskList> GetAllTaskList(Boolean includeDeleted)
        {
            var result = new List<LocalTaskList>();
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT * FROM TaskList ";
                    if (!includeDeleted)
                    {
                        command.CommandText += " AND LocalDelete = 0 ";
                    }
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(BuildTaskList(reader));
                        }
                    }
                }
            }
            return result;
        }

        public LocalTaskList InsertTaskList(LocalTaskList taskList)
        {
            LocalTaskList result = null;
            using (var connection = new SQLiteConnection(ConnectString))
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
                    Log.Info("Local Insert TaskList " + result);
                    //Insert WindowInfo
                    command.Parameters.Clear();
                    command.CommandText = @"INSERT INTO WindowInfo VALUES (null,@TaskListId,0,0,0,0,0,0);";
                    command.Parameters.Add(new SQLiteParameter("@TaskListId", localId));
                    command.ExecuteNonQuery();
                    Log.Info("Local Insert WindowInfo");
                }
            }
            return result;
        }

        public LocalTaskList UpdateTaskList(LocalTaskList taskList)
        {
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"UPDATE TaskList SET LocalModify = 1,LocalDelete = @LocalDelete,Id = @Id,Title = @Title WHERE LocalId = @LocalId;";
                    command.Parameters.Add(new SQLiteParameter("@LocalId", taskList.LocalId));
                    command.Parameters.Add(new SQLiteParameter("@LocalDelete", taskList.LocalDelete));
                    command.Parameters.Add(new SQLiteParameter("@Id", taskList.Id));
                    command.Parameters.Add(new SQLiteParameter("@Title", taskList.Title));
                    command.ExecuteNonQuery();
                }
            }
            var result = taskList.Clone();
            result.LocalModify = true;
            Log.Info("Local Update TaskList " + taskList);
            return result;
        }

        public void DeleteTaskList(LocalTaskList taskList)
        {
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"DELETE FROM TaskList WHERE LocalId = @LocalId;
                                            DELETE FROM Task WHERE LocalTaskListId = @LocalId;
                                            DELETE FROM WindowInfo WHERE TaskListId = @LocalId;";
                    command.Parameters.Add(new SQLiteParameter("@LocalId", taskList.LocalId));
                    command.ExecuteNonQuery();
                }
            }
            Log.Info("Local Delete TaskList " + taskList);
        }

        public LocalTaskList DeleteTaskListLogic(LocalTaskList taskList)
        {
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"UPDATE TaskList SET LocalDelete = 1 WHERE LocalId = @LocalId;
                                            UPDATE Task SET LocalDelete = 1 WHERE LocalTaskListId = @LocalId;
                                            DELETE FROM WindowInfo WHERE TaskListId = @LocalId;";
                    command.Parameters.Add(new SQLiteParameter("@LocalId", taskList.LocalId));
                    command.ExecuteNonQuery();
                }
            }
            var result = taskList.Clone();
            result.LocalDelete = true;
            Log.Info("Local Logic Delete TaskList " + taskList);
            return result;
        }

        public void ClearTaskList()
        {
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"UPDATE TaskList SET LocalModify = 0;DELETE FROM TaskList WHERE LocalDelete = 1;";
                    command.ExecuteNonQuery();
                }
            }
            Log.Info("Local Clear TaskList");
        }

        private static LocalTask BuildTask(SQLiteDataReader reader)
        {
            return new LocalTask
            {
                LocalId = (Int64)reader["LocalId"],
                LocalModify = (Boolean)reader["LocalModify"],
                LocalTaskListId = (Int64)reader["LocalTaskListId"],
                LocalDelete = (Boolean)reader["LocalDelete"],
                Id = CastToString(reader["Id"]),
                Title = CastToString(reader["Title"]),
                ETag = CastToString(reader["ETag"]),
                Due = CastToDateTime(reader["Due"]),
                Notes = CastToString(reader["Notes"]),
                Parent = CastToString(reader["Parent"]),
                Position = CastToString(reader["Position"]),
                Status = CastToString(reader["Status"]),
            };
        }

        public List<LocalTask> GetTasksByList(LocalTaskList list,Boolean includeDeleted)
        {
            var result = new List<LocalTask>();
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT * FROM Task WHERE LocalTaskListId = @LocalTaskListId ";
                    if (!includeDeleted)
                    {
                        command.CommandText += " AND LocalDelete = 0 ";
                    }
                    command.Parameters.Add(new SQLiteParameter("@LocalTaskListId", list.LocalId));
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(BuildTask(reader));
                        }
                    }
                }
            }
            return result;
        }

        public LocalTask InsertTask(LocalTask task)
        {
            LocalTask result = null;
            using (var connection = new SQLiteConnection(ConnectString))
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
            Log.Info("Local Insert Task " + result);
            return result;
        }

        public LocalTask UpdateTask(LocalTask task)
        {
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText =
@"UPDATE Task
SET LocalTaskListId = @LocalTaskListId ,LocalModify = 1,LocalDelete = @LocalDelete,Id = @Id,Title = @Title,
ETag = @ETag, Due = @Due, Notes = @Notes, Parent = @Parent, Position = @Position, Status = @Status
WHERE LocalId = @LocalId;";
                    command.Parameters.Add(new SQLiteParameter("@LocalId", task.LocalId));
                    command.Parameters.Add(new SQLiteParameter("@LocalTaskListId", task.LocalTaskListId));
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
            var result = task.Clone();
            result.LocalModify = true;
            Log.Info("Local Update Task " + task);
            return result;
        }

        public void DeleteTask(LocalTask task)
        {
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"DELETE FROM Task WHERE LocalId = @LocalId;";
                    command.Parameters.Add(new SQLiteParameter("@LocalId", task.LocalId));
                    command.ExecuteNonQuery();
                }
            }
            Log.Info("Local Delete Task " + task);
        }

        public LocalTask DeleteTaskLogic(LocalTask task)
        {
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"UPDATE Task SET LocalDelete = 1 WHERE LocalId = @LocalId;";
                    command.Parameters.Add(new SQLiteParameter("@LocalId", task.LocalId));
                    command.ExecuteNonQuery();
                }
            }
            Log.Info("Local Logic Delete Task " + task);
            var result = task.Clone();
            result.LocalDelete = true;
            return result;
        }

        public void ClearTask()
        {
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"UPDATE Task SET LocalModify = 0;DELETE FROM Task WHERE LocalDelete = 1;";
                    command.ExecuteNonQuery();
                }
            }
            Log.Info("Local Clear Task");
        }

        private static WindowInfo BuildWindowInfo(SQLiteDataReader reader)
        {
            return new WindowInfo
            {
                Id = (Int64)reader["Id"],
                TaskListId = (Int64)reader["TaskListId"],
                Left = (Double)reader["Left"],
                Top = (Double)reader["Top"],
                Height = (Double)reader["Height"],
                Width = (Double)reader["Width"],
                IsHideWindow = (Boolean)reader["IsHideWindow"],
                IsShowCompleted = (Boolean)reader["IsShowCompleted"]
            };
        }

        public List<WindowInfo> GetAllWindowInfo(Boolean includeHide)
        {
            var result = new List<WindowInfo>();
            using(var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using(var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT * FROM WindowInfo ";
                    if(!includeHide)
                    {
                        command.CommandText += " WHERE IsHideWindow = 0 ";
                    }
                    using(var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            result.Add(BuildWindowInfo(reader));
                        }
                    }
                }
            }
            return result;
        } 

        public WindowInfo GetWindowInfoByTaskList(LocalTaskList list)
        {
            WindowInfo result = null;
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = @"SELECT * FROM WindowInfo WHERE TaskListId = @TaskListId ";
                    command.Parameters.Add(new SQLiteParameter("@TaskListId", list.LocalId));
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = BuildWindowInfo(reader);
                        }
                    }
                }
            }
            return result;
        }

        public void UpdateWindowInfo(WindowInfo winInfo)
        {
            using (var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText =
@"UPDATE WindowInfo
SET Left = @Left ,Top = @Top,Height = @Height,Width = @Width,IsHideWindow = @IsHideWindow,IsShowCompleted = @IsShowCompleted
WHERE Id = @Id;";
                    command.Parameters.Add(new SQLiteParameter("@Id", winInfo.Id));
                    command.Parameters.Add(new SQLiteParameter("@Left", winInfo.Left));
                    command.Parameters.Add(new SQLiteParameter("@Top", winInfo.Top));
                    command.Parameters.Add(new SQLiteParameter("@Height", winInfo.Height));
                    command.Parameters.Add(new SQLiteParameter("@Width", winInfo.Width));
                    command.Parameters.Add(new SQLiteParameter("@IsHideWindow", winInfo.IsHideWindow));
                    command.Parameters.Add(new SQLiteParameter("@IsShowCompleted", winInfo.IsShowCompleted));
                    command.ExecuteNonQuery();
                }
            }
        }

        private static ManagerModel BuildManagerModel(SQLiteDataReader reader)
        {
            return new ManagerModel
            {
                ListId = (Int64)reader["ListId"],
                Title = CastToString(reader["Title"]),
                IsHideWindow = (Boolean)reader["IsHideWindow"],
            };
        }

        public List<ManagerModel> GetAllManagerModel()
        {
            var result = new List<ManagerModel>();
            using(var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using(var command = new SQLiteCommand(connection))
                {
                    command.CommandText =
@"select l.LocalId as ListId,l.Title as Title,w.IsHideWindow as IsHideWindow
from TaskList l,WindowInfo w
where w.TaskListId = l.LocalId and l.LocalDelete = 0";
                    using(var reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            result.Add(BuildManagerModel(reader));
                        }
                    }
                }
            }
            return result;
        }

        public void UpdateManagerModel(ManagerModel model)
        {
            using(var connection = new SQLiteConnection(ConnectString))
            {
                connection.Open();
                using(var command = new SQLiteCommand(connection))
                {
                    command.CommandText =@"update TaskList set Title = @Title where LocalId = @ListId;
                                           update WindowInfo set IsHideWindow = @IsHideWindow where TaskListId = @ListId;";
                    command.Parameters.Add(new SQLiteParameter("@Title", model.Title));
                    command.Parameters.Add(new SQLiteParameter("@ListId", model.ListId));
                    command.Parameters.Add(new SQLiteParameter("@IsHideWindow", model.IsHideWindow));
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
