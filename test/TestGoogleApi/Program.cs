using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace TestGoogleApi
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Tasks OAuth2 Sample");
            Console.WriteLine("===================");

            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { TasksService.Scope.Tasks },
                    "user", CancellationToken.None, new FileDataStore("Tasks.Auth.Store")).Result;
            }

            var service = new TasksService(new BaseClientService.Initializer()
            {
                HttpClientFactory = new ProxyableHttpClientFactory("http://127.0.0.1:8087"),
                HttpClientInitializer = credential
            });

//            var list = service.Tasklists.Get("MDE4Nzg2MTYzODcwNzU2NzUyNzU6MTc0NjU1NDM3Mjow").Execute();
//            var localTask = new Task() {Title = "new"};
//            var remoteTask = service.Tasks.Insert(localTask, list.Id).Execute();
//
//            Tasks tasks = service.Tasks.List(list.Id).Execute();
//            foreach(Task task in tasks.Items)
//            {
//                Console.WriteLine("{0}|{1}|{2}|{3}", task.Title, task.Id, task.Parent, task.Position);
//            }
            
            TaskLists taskLists = service.Tasklists.List().Execute();
            foreach (TaskList list in taskLists.Items)
            {
                Console.WriteLine("{0}|{1}",list.Title,list.Id);
                Tasks tasks = service.Tasks.List(list.Id).Execute();
                foreach(Task task in tasks.Items)
                {
                    Console.WriteLine("{0}|{1}", task.Title, task.ETag);
                }
            }

            Console.ReadKey();
        }
    }
}
