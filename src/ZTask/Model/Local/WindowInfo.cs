using System;

namespace ZTask.Model.Local
{
    public class WindowInfo
    {
        public virtual Int64 Id { get; set; }
        public virtual Int64 TaskListId { get; set; }

        public virtual Double Left { get; set; }
        public virtual Double Top { get; set; }
        public virtual Double Height { get; set; }
        public virtual Double Width { get; set; }

        public virtual Boolean IsHideWindow { get; set; }
        public virtual Boolean IsShowCompleted { get; set; }

    }
}
