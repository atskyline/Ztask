using System;
using ZTask.Model.Core;

namespace ZTask.Model.Interface
{
    public interface IDataService
    {
        void GetData(Action<DataItem, Exception> callback);
    }
}
