using System;
using Google.Apis.Tasks.v1.Data;

namespace TestSync.Local
{
    class LocalTaskList : TaskList, ICloneable
    {
        public virtual Int64 LocalId { get; set; }
        public virtual Boolean LocalModify { get; set; }
        public virtual Boolean LocalDelete { get; set; }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
        public LocalTaskList Clone()
        {
            return (LocalTaskList)this.MemberwiseClone();
        }

        public override string ToString()
        {
            return String.Format("{0}[{1}]", Title, Id);
        }
    }
}
