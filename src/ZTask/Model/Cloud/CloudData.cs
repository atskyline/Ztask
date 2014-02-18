using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using Google.Apis.Util.Store;
using log4net;

namespace ZTask.Model.Cloud
{
    public class CloudData : IDisposable
    {
        private static readonly ILog Log = Util.Log;
        private static readonly AppConfig Config = AppConfig.Load();

        private const String ApiClientId = "603900121266-b03b2m3481nd76ql0n32phtvtu4ii05t.apps.googleusercontent.com";
        private const String ApiClientSecret = "cYKpt8jk5WJQDeXLPJ_Brzob";

        private TasksService _taskService;

        public void Authorize()
        {
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            new ClientSecrets { ClientId = ApiClientId, ClientSecret = ApiClientSecret },
                            new[] { TasksService.Scope.Tasks },
                            "user", CancellationToken.None, new FileDataStore("Tasks.Auth.Store")).Result;
            _taskService = new TasksService(new BaseClientService.Initializer()
            {
                HttpClientFactory = String.IsNullOrEmpty(Config.Proxy) ? new HttpClientFactory() : new ProxyableHttpClientFactory(Config.Proxy),
                HttpClientInitializer = credential,
            });
            Log.Info("Get TasksService Success");
        }

        public List<TaskList> GetAllTaskList()
        {
            return _taskService.Tasklists.List().Execute().Items.ToList();
        }

        public TaskList InserTaskList(TaskList taskList)
        {
            var result = _taskService.Tasklists.Insert(taskList).Execute();
            Log.InfoFormat("Cloud Insert TaskList {0}[{1}]", taskList.Title, taskList.Id);
            return result;
        }

        public TaskList UpdateTaskList(TaskList taskList)
        {
            var result = _taskService.Tasklists.Update(taskList, taskList.Id).Execute();
            Log.InfoFormat("Cloud Update TaskList {0}[{1}]", taskList.Title, taskList.Id);
            return result;
        }

        public void DeleteTaskList(TaskList taskList)
        {
            _taskService.Tasklists.Delete(taskList.Id).Execute();
            Log.InfoFormat("Cloud Delete TaskList {0}[{1}]", taskList.Title, taskList.Id);
        }

        public List<Task> GetTaskByList(TaskList list)
        {
            return _taskService.Tasks.List(list.Id).Execute().Items.ToList();
        }

        public Task InserTask(Task task, TaskList list)
        {
            var result = _taskService.Tasks.Insert(task, list.Id).Execute();
            Log.InfoFormat("Cloud Insert Task {0}[{1}]", task.Title, task.Id);
            return result;
        }

        public Task UpdateTask(Task task, TaskList list)
        {
            var result = _taskService.Tasks.Update(task, list.Id, task.Id).Execute();
            Log.InfoFormat("Cloud Update Task {0}[{1}]", task.Title, task.Id);
            return result;
        }

        public void DeleteTask(Task task, TaskList list)
        {
            _taskService.Tasks.Delete(list.Id, task.Id).Execute();
            Log.InfoFormat("Cloud Delete Task {0}[{1}]", task.Title, task.Id);
        }

        public void Dispose()
        {
            if (_taskService != null)
            {
                _taskService.Dispose();
            }
        }
    }
}