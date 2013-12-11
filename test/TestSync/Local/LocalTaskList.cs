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
        public LocalTaskList Clone(TaskList list)
        {
            var clone = new LocalTaskList();
            clone.LocalId = this.LocalId;
            clone.LocalModify = this.LocalModify;
            clone.LocalDelete = this.LocalDelete;
            clone.ETag = list.ETag;
            clone.Id = list.Id;
            clone.Kind = list.Kind;
            clone.SelfLink = list.SelfLink;
            clone.Title = list.Title;
            clone.Updated = list.Updated;
            return clone;
        }

        public override string ToString()
        {
            return String.Format("{0}[{1}]", Title, Id);
        }
    }
}
