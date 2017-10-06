using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SGcombo.TaskContainer
{

    ////////////////////////////////////////////////////////////////////////////
    //	Copyright 2017 : Vladimir Novick    https://www.linkedin.com/in/vladimirnovick/  
    //        
    //             https://github.com/Vladimir-Novick/TaskContainer-Manager
    //
    //    NO WARRANTIES ARE EXTENDED. USE AT YOUR OWN RISK. 
    //
    // To contact the author with suggestions or comments, use  :vlad.novick@gmail.com
    //
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    ///    Running multiple tasks asynchronously
    /// </summary>
    public class TaskContainerManager
    {

        public TaskContainerManager()
        {

        }

        private class TaskItem
        {
            public String taskName { get; set; }
            public int Id { get; set; }
            public Task task_ { get; set; }

            public static bool operator == (TaskItem taskItem, Task task)
            {
                return
                    taskItem.task_.Id == task.Id;
            }

            public static bool operator !=(TaskItem taskItem, Task task)
            {
                return
                    taskItem.task_.Id != task.Id;
            }

            public override bool Equals(Object task)
            {
                Task item = task as Task;
                if (item != null)
                {
                    return this.task_.Id == item.Id;
                }
                TaskItem taskItem = task as TaskItem;
                if (taskItem != null)
                {
                    return this.task_.Id == taskItem.task_.Id;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return this.task_.GetHashCode();
            }

        }

        public void WaitAll()
        {
            while (TasksContainer.Count > 0)
            {
                try
                {
                    List<Task> TaskList = new List<Task>();
                    foreach (var item in TasksContainer.Values)
                    {
                        TaskList.Add(item.task_);
                    }

                    Task.WaitAny(TaskList.ToArray());
                }
                catch { }
            }

        }

        private ConcurrentDictionary<int, TaskItem> TasksContainer = new ConcurrentDictionary<int, TaskItem>();

        /// <summary>
        ///    Get Task Count
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return TasksContainer.Count();
        }

        /// <summary>
        ///   Check specific task is completed
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsCompleted(String name)
        {
            try
            {
                if (TasksContainer.Count > 0)
                {
                    TaskItem item = TasksContainer.Values.FirstOrDefault(x => x.taskName == name);
                    if (item != null)
                    {
                        return false;
                    }
                }
            }
            catch { }
            return true;
        }
        /// <summary>
        ///   Get task status by task name
        /// </summary>
        /// <param name="TaskName"></param>
        /// <returns></returns>
        public TaskStatus Status(String TaskName)
        {
            try
            {
                if (TasksContainer.Count > 0)
                {
                    TaskItem item = TasksContainer.Values.FirstOrDefault(x => x.taskName == TaskName);
                    if (item != null)
                    {
                        Task task = item.task_;
                        if (task == null) return TaskStatus.RanToCompletion;
                        return task.Status;
                    }
                }
            }
            catch { }
            return TaskStatus.RanToCompletion;
        }

        /// <summary>
        ///    Add a task to container
        /// </summary>
        /// <param name="task"></param>
        /// <param name="taskName"></param>
        /// <returns>true/false - task is active</returns>
        public bool TryAdd(Task task, String taskName = null)
        {
            task.ContinueWith(t1 =>
            {
                try
                {
                    TaskItem outItem;
                    TasksContainer.TryRemove(t1.Id, out outItem);
                    t1.Dispose();
                }
                catch (Exception ex)
                { 

                }
            });
            String TaskName = task.Id.ToString();
            int Task_ID = task.Id;
            if (taskName != null)
            {
                TaskName = taskName;
            }
            var taskItem = new TaskItem
            {
                taskName = TaskName,
                Id = task.Id,
                task_ = task
            };

            bool ok = TasksContainer.TryAdd(taskItem.Id, taskItem);

            if (ok)
            {
                task.Start();

                return true;
            }
            return false;
        }

    }
}
