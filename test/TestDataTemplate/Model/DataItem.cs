using System;

namespace TestDataTemplate.Model
{
    public class DataItem
    {
        public virtual String Id { get; set; }
        public virtual String Status { get; set; }
        public virtual String Title { get; set; }

        public virtual Boolean IsCompleted
        {
            get
            {
                return Status == "completed";
            }

            set 
            {
                Status = value == true ? "completed" : "needsAction";
            }
        }
    }
}
