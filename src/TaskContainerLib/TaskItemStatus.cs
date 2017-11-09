using System;
using System.Threading.Tasks;

namespace TaskContainerLib
{
    public class TaskItemStatus
    {
        public String Name { get; set; }
        public String Description { get; set; }
        public DateTime Start { get; set; }

        public TaskStatus Status { get; set; }

    }
}
