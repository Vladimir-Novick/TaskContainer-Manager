using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskContainerLib
{
    public class TaskItem
    {
        public String TaskName { get; set; }
        public int Id { get; set; }
        public Task Task_ { get; set; }

        public String Description { get; set; }

        public String CurrentStatus { get; set; }

        public DateTime StartTime { get; set; }
        private int HashCode { get; set; }
        public int MaxTime { get; internal set; }



        /// <summary>
        ///    Callback functions 
        /// </summary>
        public Func<String, bool> Callback;

        public static bool operator ==(TaskItem taskItem, Task task)
        {
            if ((task == null) && (taskItem as Object != null)) return false;
            if ((task == null) && (taskItem as Object == null)) return true;
            if (!(task is Task)) return false;
            return
                taskItem.Id == task.Id;
        }


        public static bool operator !=(TaskItem taskItem, Task task)
        {

            if ((task == null) && (taskItem as Object != null)) return true;
            if ((task == null) && (taskItem as Object == null)) return false;

            if (!(task is Task)) return true;
            return
                taskItem.Id != task.Id;
        }

        public override bool Equals(Object task)
        {
            if (!(task is Task)) return false;
            if (task == null) return false;
            Task item = task as Task;
            if (item != null)
            {
                return this.Id == item.Id;
            }
            TaskItem taskItem = task as TaskItem;
            if (taskItem != null)
            {
                return this.Id == taskItem.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Task_.GetHashCode();
        }

    }
}
