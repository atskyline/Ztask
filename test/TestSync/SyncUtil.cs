using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Tasks.v1.Data;
using log4net;
using TestSync.Local;

namespace TestSync
{
    class SyncUtil
    {
        private static readonly ILog LOG = Util.GetLog();

        private LocalData _localData;
        private CloudData _cloudData;
        /// <summary>
        /// 同步规则
        /// Cloud Insert : Local.Id = null
        /// Cloud Update : Local.Id != null && Local.LocalModify == true
        /// Cloud Delete : Local.LocalDelete == true
        /// Local Insert : Cloud - Local
        /// Local Update : Cloud.Etag != Local.Etag
        /// Local Delete : (Local - Cloud) && Local.Id != null
        /// </summary>
        public void Sync()
        {
            _localData = LocalData.getInstance();
            _cloudData = new CloudData();
            _cloudData.Authorize();
            SyncTaskList(_cloudData.GetAllTaskList(),_localData.GetAllTaskList());
        }

        private void SyncTaskList(List<TaskList> cloudTaskLists, List<LocalTaskList> localTaskLists)
        {
            LOG.Info("Sync TaskList Cloud Insert");
            localTaskLists.ForEach((list) =>
            {
                if (String.IsNullOrEmpty(list.Id))
                {
                    var newCloudList = _cloudData.InserTaskList(list);
                    var newLocalList = list.Clone();
                    newLocalList.Id = newCloudList.Id;
                    _localData.UpdateTaskList(newLocalList);
                }
            });
            LOG.Info("Sync TaskList Cloud Update");
            localTaskLists.ForEach((list) =>
            {
                if (!String.IsNullOrEmpty(list.Id) && list.LocalModify == true)
                {
                    _cloudData.UpdateTaskList(list);
                }
            });
            LOG.Info("Sync TaskList Cloud Delete");
            localTaskLists.ForEach((list) =>
            {
                if (list.LocalDelete == true)
                {
                    _cloudData.DeleteTaskList(list);
                }
            });
            LOG.Info("Sync TaskList Local Insert");
            cloudTaskLists.Except(localTaskLists,new CloudTaskListIdComparer()).ToList().ForEach((list) =>
            {
                _localData.InsertTaskList(new LocalTaskList().Clone(list));
            });
            LOG.Info("Sync TaskList Local Update");
            cloudTaskLists.Join(localTaskLists,cloudList=>cloudList.Id,localList=>localList.Id,
                (cloudList, localList) => new { CloudList = cloudList, LocalList = localList }).ToList().ForEach((list) =>
            {
                if (list.CloudList.ETag != list.LocalList.ETag)
                {
                    var newLocalList = list.LocalList.Clone(list.CloudList);
                    _localData.UpdateTaskList(newLocalList);
                }
            });
            LOG.Info("Sync TaskList Local Delete");
            localTaskLists.Except(cloudTaskLists,new CloudTaskListIdComparer()).ToList().ForEach((list) =>
            {
                if (list.Id != null)
                {
                    _localData.DeleteTaskList((LocalTaskList)list);
                }
            });
            LOG.Info("Sync TaskList Local Clear");
            _localData.ClearTaskList();
        }

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
    }
}
