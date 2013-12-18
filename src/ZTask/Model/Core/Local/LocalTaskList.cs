using System;
using Google.Apis.Tasks.v1.Data;

namespace ZTask.Model.Core.Local
{
    public class LocalTaskList : TaskList, ICloneable
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
            var clone = new LocalTaskList
            {
                LocalId = this.LocalId,
                LocalModify = this.LocalModify,
                LocalDelete = this.LocalDelete,
                ETag = list.ETag,
                Id = list.Id,
                Kind = list.Kind,
                SelfLink = list.SelfLink,
                Title = list.Title,
                Updated = list.Updated
            };
            return clone;
        }

        public override string ToString()
        {
            return String.Format("{0}[{1}]", Title, Id);
        }
    }
}
