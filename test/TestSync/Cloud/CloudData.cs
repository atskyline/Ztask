using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using Google.Apis.Util.Store;
using log4net;

namespace TestSync.Local
{
    public class CloudData
    {
        private static readonly ILog LOG = Util.GetLog();

        private const String ApiClientId = "603900121266-b03b2m3481nd76ql0n32phtvtu4ii05t.apps.googleusercontent.com";
        private const String ApiClientSecret = "cYKpt8jk5WJQDeXLPJ_Brzob";
        private const String Proxy = "http://127.0.0.1:8087";

        private UserCredential credential;
        private TasksService taskService;

        public void Authorize()
        {
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            new ClientSecrets { ClientId = ApiClientId, ClientSecret = ApiClientSecret },
                            new[] { TasksService.Scope.Tasks },
                            "user", CancellationToken.None, new FileDataStore("Tasks.Auth.Store")).Result;
            taskService = new TasksService(new BaseClientService.Initializer()
            {
                HttpClientFactory = new ProxyableHttpClientFactory(Proxy),
                HttpClientInitializer = credential,
            });
            LOG.Debug("Get TasksService Success");
        }

        public List<TaskList> GetAllTaskList()
        {
            return taskService.Tasklists.List().Execute().Items.ToList();
        }

        public TaskList InserTaskList(TaskList taskList)
        {
            var result = taskService.Tasklists.Insert(taskList).Execute();
            LOG.InfoFormat("Cloud Insert TaskList {0}[{1}]", taskList.Title, taskList.Id);
            return result;
        }

        public TaskList UpdateTaskList(TaskList taskList)
        {
            var result = taskService.Tasklists.Update(taskList, taskList.Id).Execute();
            LOG.InfoFormat("Cloud Update TaskList {0}[{1}]", taskList.Title, taskList.Id);
            return result;
        }

        public void DeleteTaskList(TaskList taskList)
        {
            taskService.Tasklists.Delete(taskList.Id).Execute();
            LOG.InfoFormat("Cloud Delete TaskList {0}[{1}]", taskList.Title, taskList.Id);
        }

        public List<Task> GetTaskByList(TaskList list)
        {
            return taskService.Tasks.List(list.Id).Execute().Items.ToList();
        }

        public Task InserTask(Task task, TaskList list)
        {
            var result = taskService.Tasks.Insert(task, list.Id).Execute();
            LOG.InfoFormat("Cloud Insert Task {0}[{1}]", task.Title, task.Id);
            return result;
        }

        public Task UpdateTask(Task task, TaskList list)
        {
            var result = taskService.Tasks.Update(task, list.Id, task.Id).Execute();
            LOG.InfoFormat("Cloud Update Task {0}[{1}]", task.Title, task.Id);
            return result;
        }

        public void DeleteTask(Task task, TaskList list)
        {
            taskService.Tasks.Delete(list.Id, task.Id).Execute();
            LOG.InfoFormat("Cloud Delete Task {0}[{1}]", task.Title, task.Id);
        }
    }
}