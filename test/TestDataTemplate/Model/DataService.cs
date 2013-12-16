using System;
using System.Collections.Generic;

namespace TestDataTemplate.Model
{
    public class DataService : IDataService
    {
        public List<DataItem> List()
        {
            return new List<DataItem>
            {
                new DataItem(){Title = "Item 1"},
                new DataItem(){Title = "Item 2"},
                new DataItem(){Title = "Item 3"},
                new DataItem(){Title = "Item 4"},
                new DataItem(){Title = "Item 5"}
            };
        }
    }
}