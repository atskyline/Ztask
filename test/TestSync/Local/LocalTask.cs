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

        public LocalTask()
        {
            this.Status = "needsAction";
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
            var clone = new LocalTask();
            clone.LocalId = this.LocalId;
            clone.LocalTaskListId = this.LocalTaskListId;
            clone.LocalModify = this.LocalModify;
            clone.LocalDelete = this.LocalDelete;
            clone.Completed = task.Completed;
            clone.Deleted = task.Deleted;
            clone.Due = task.Due;
            clone.ETag = task.ETag;
            clone.Hidden = task.Hidden;
            clone.Id = task.Id;
            clone.Kind = task.Kind;
            clone.Links = task.Links;
            clone.Notes = task.Notes;
            clone.Parent = task.Parent;
            clone.Position = task.Position;
            clone.SelfLink = task.SelfLink;
            clone.Status = task.Status;
            clone.Title = task.Title;
            clone.Updated = task.Updated;
            return clone;
        }

        public override string ToString()
        {
            return String.Format("{0}[{1}]", Title, Id);
        }

        public virtual void MarkCompleted()
        {
            LocalModify = true;
            Status = "completed";
        }

        public virtual void MarkNeedsAction()
        {
            LocalModify = true;
            Status = "needsAction";  
        }

        public virtual void ToggleStatus()
        {
            LocalModify = true;
            Status = Status == "completed" ? "needsAction" : "completed";
        }

        public virtual Boolean IsCompleted()
        {
            return Status == "completed";
        }
    }
}