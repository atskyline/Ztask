using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Tasks.v1.Data;
using log4net;
using ZTask.Model.Core.Cloud;
using ZTask.Model.Core.Local;

namespace ZTask.Model.Core
{
    class SyncUtil
    {
        private static readonly ILog Log = Util.Log;

        private LocalData _localData;
        private CloudData _cloudData;
        /// <summary>
        /// 同步规则
        /// Local Insert : Cloud - Local
        /// Local Update : Cloud.Etag != Local.Etag {CloudList.Title != LocalList.Title}
        /// Local Delete : (Local - Cloud) && Local.Id != null
        /// Cloud Insert : Local.Id = null && Local.LocalDelete == false
        /// Cloud Update : Local.Id != null && Local.LocalModify == true && Local.LocalDelete == fase
        /// Cloud Delete : Local.Id != null && Local.LocalDelete == true
        /// </summary>
        public void Sync()
        {
            using (_cloudData = new CloudData())
            {
                _localData = LocalData.Instance;
                _cloudData.Authorize();
                SyncAllTaskLists(_cloudData.GetAllTaskList(), _localData.GetAllTaskList());
                _localData.GetAllTaskList().ForEach((list) =>
                {
                    SyncTaskList(list);
                });
            }
        }

        private void SyncAllTaskLists(List<TaskList> cloudTaskLists, List<LocalTaskList> localTaskLists)
        {
            Log.Info("Start Sync TaskLists");
            Log.Info("Sync TaskList Local Insert");
            cloudTaskLists.Except(localTaskLists, new CloudTaskListIdComparer()).ToList().ForEach((list) =>
            {
                _localData.InsertTaskList(new LocalTaskList().Clone(list));
            });
            Log.Info("Sync TaskList Local Update");
            cloudTaskLists.Join(localTaskLists, cloudList => cloudList.Id, localList => localList.Id,
                (cloudList, localList) => new { CloudList = cloudList, LocalList = localList }).ToList().ForEach((list) =>
                {
                    if (list.CloudList.Title != list.LocalList.Title)
                    {
                        var newLocalList = list.LocalList.Clone(list.CloudList);
                        _localData.UpdateTaskList(newLocalList);
                    }
                });
            Log.Info("Sync TaskList Local Delete");
            localTaskLists.Except(cloudTaskLists, new CloudTaskListIdComparer()).ToList().ForEach((list) =>
            {
                if (list.Id != null)
                {
                    _localData.DeleteTaskList((LocalTaskList)list);
                }
            });
            Log.Info("Sync TaskList Cloud Insert");
            localTaskLists.ForEach((list) =>
            {
                if (String.IsNullOrEmpty(list.Id) && list.LocalDelete == false)
                {
                    var newCloudList = _cloudData.InserTaskList(list);
                    var newLocalList = list.Clone(newCloudList);
                    _localData.UpdateTaskList(newLocalList);
                }
            });
            Log.Info("Sync TaskList Cloud Update");
            localTaskLists.ForEach((list) =>
            {
                if (!String.IsNullOrEmpty(list.Id) && list.LocalModify == true && list.LocalDelete == false)
                {
                    _cloudData.UpdateTaskList(list);
                }
            });
            Log.Info("Sync TaskList Cloud Delete");
            localTaskLists.ForEach((list) =>
            {
                if (list.Id != null && list.LocalDelete == true)
                {
                    _cloudData.DeleteTaskList(list);
                }
            });
            Log.Info("Sync TaskList Local Clear");
            _localData.ClearTaskList();
        }


        private void SyncTaskList(LocalTaskList list)
        {
            Log.Info("Start Sync TaskList: " + list);
            var localTasks = _localData.GetTasksByList(list);
            var cloudTasks = _cloudData.GetTaskByList(list);

            Log.Info("Sync Task Local Insert");
            cloudTasks.Except(localTasks, new CloudTaskIdComparer()).ToList().ForEach((task) =>
            {
                var newLocalTask = new LocalTask().Clone(task);
                newLocalTask.LocalTaskListId = list.LocalId;
                _localData.InsertTask(newLocalTask);
            });
            Log.Info("Sync Task Local Update");
            cloudTasks.Join(localTasks, cloudTask => cloudTask.Id, localTask => localTask.Id,
                (cloudTask, localTask) => new { CloudTask = cloudTask, LocalTask = localTask }).ToList().ForEach((task) =>
                {
                    if (task.CloudTask.ETag != task.LocalTask.ETag)
                    {
                        var newLocalTask = task.LocalTask.Clone(task.CloudTask);
                        _localData.UpdateTask(newLocalTask);
                    }
                });
            Log.Info("Sync Task Local Delete");
            localTasks.Except(cloudTasks, new CloudTaskIdComparer()).ToList().ForEach((task) =>
            {
                if (task.Id != null)
                {
                    _localData.DeleteTask((LocalTask)task);
                }
            });
            Log.Info("Sync Task Cloud Insert");
            localTasks.ForEach((task) =>
            {
                if (String.IsNullOrEmpty(task.Id) && task.LocalDelete == false)
                {
                    var newCloudTask = _cloudData.InserTask(task,list);
                    var newLocalTask = task.Clone(newCloudTask);
                    _localData.UpdateTask(newLocalTask);
                }
            });
            Log.Info("Sync Task Cloud Update");
            localTasks.ForEach((task) =>
            {
                if (!String.IsNullOrEmpty(task.Id) && task.LocalModify == true)
                {
                    _cloudData.UpdateTask(task,list);
                }
            });
            Log.Info("Sync Task Cloud Delete");
            localTasks.ForEach((task) =>
            {
                if (task.Id != null && task.LocalDelete == true && task.LocalDelete == false)
                {
                    _cloudData.DeleteTask(task,list);
                }
            });
            Log.Info("Sync Task Local Clear");
            _localData.ClearTask();
        }

        /// <summary>
        /// 根据Id判断TaskList的相等性
        /// </summary>
        private class CloudTaskListIdComparer : IEqualityComparer<TaskList>
        {
            public bool Equals(TaskList x, TaskList y)
            {
                return x.Id != null && y.Id != null && x.Id == y.Id;
            }

            public int GetHashCode(TaskList obj)
            {

                return obj.Id == null ? 0 : obj.Id.GetHashCode();
            }
        }

        /// <summary>
        /// 根据Id判断Task的相等性
        /// </summary>
        private class CloudTaskIdComparer : IEqualityComparer<Task>
        {
            public bool Equals(Task x, Task y)
            {
                return x.Id != null && y.Id != null && x.Id == y.Id;
            }

            public int GetHashCode(Task obj)
            {

                return obj.Id == null ? 0 : obj.Id.GetHashCode();
            }
        }
    }
}
