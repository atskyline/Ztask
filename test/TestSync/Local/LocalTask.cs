using System;
using Google.Apis.Tasks.v1.Data;

namespace TestSync.Local
{
    public class LocalTask :Task,ICloneable
    {
        public virtual Int64 LocalId { get; set; }
        public virtual Boolean LocalModify { get; set; }
        public virtual Boolean LocalDelete { get; set; }
        public virtual Int64 LocalTaskListId { get; set; }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
        public LocalTask Clone()
        {
            return (LocalTask)this.MemberwiseClone();
        }

        public override string ToString()
        {
            return String.Format("{0}[{1}]", Title, Id);
        }

        public  virtual Boolean IsCompleted()
        {
            return Status == "completed";
        }
    }
}