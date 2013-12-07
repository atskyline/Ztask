using System;

namespace TestSqlite
{
    class TaskList : ICloneable
    {
        public virtual Int64 Id { get; set; }
        public virtual String Title { get; set; }


        object ICloneable.Clone()
        {
            return this.Clone();
        }
        public TaskList Clone()
        {
            return (TaskList)this.MemberwiseClone();
        }
    }
}
