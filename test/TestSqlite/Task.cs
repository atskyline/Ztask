using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSqlite
{
    class Task
    {
        public virtual Int64 Id { get; set; }
        public virtual String Title { get; set; }
        public virtual String Completed { get; set; }

        public virtual Int64 TaskLiskId { get; set; }
    }
}
