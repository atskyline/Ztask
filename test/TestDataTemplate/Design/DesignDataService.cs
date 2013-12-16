using System.Collections.Generic;
using TestDataTemplate.Model;

namespace TestDataTemplate.Design
{
    public class DesignDataService : IDataService
    {
        public List<DataItem> List()
        {
            return new List<DataItem>
            {
                new DataItem(){Title = "Design 1"},
                new DataItem(){Title = "Design 2"},
                new DataItem(){Title = "Design 3"},
                new DataItem(){Title = "Design 4"},
                new DataItem(){Title = "Design 5"}
            };
        }
    }
}