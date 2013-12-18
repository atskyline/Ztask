using System;
using Google.Apis.Tasks.v1.Data;

namespace ZTask.Model.Core.Local
{
    public class LocalTask :Task,ICloneable
    {
        public virtual Int64 LocalId { get; set; }
        public virtual Boolean LocalModify { get; set; }
        public virtual Boolean LocalDelete { get; set; }
        public virtual Int64 LocalTaskListId { get; set; }

        public virtual Boolean IsCompleted
        {
            get
            {
                return Status == "completed";
            }
            set
            {
                Status = value ? "completed" : "needsAction";
            }
        }

        public LocalTask()
        {
            IsCompleted = false;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
        public LocalTask Clone()
        {
            return (LocalTask)this.MemberwiseClone();
        }
        public LocalTask Clone(Task task)
        {
            var clone = new LocalTask
            {
                LocalId = this.LocalId,
                LocalTaskListId = this.LocalTaskListId,
                LocalModify = this.LocalModify,
                LocalDelete = this.LocalDelete,
                Completed = task.Completed,
                Deleted = task.Deleted,
                Due = task.Due,
                ETag = task.ETag,
                Hidden = task.Hidden,
                Id = task.Id,
                Kind = task.Kind,
                Links = task.Links,
                Notes = task.Notes,
                Parent = task.Parent,
                Position = task.Position,
                SelfLink = task.SelfLink,
                Status = task.Status,
                Title = task.Title,
                Updated = task.Updated
            };
            return clone;
        }

        public override string ToString()
        {
            return String.Format("{0}[{1}]", Title, Id);
        }


    }
}