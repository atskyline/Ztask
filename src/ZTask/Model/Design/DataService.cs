using System;
using ZTask.Model.Core;
using ZTask.Model.Interface;

namespace ZTask.Model.Design
{
    public class DataService : IDataService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }
    }
}