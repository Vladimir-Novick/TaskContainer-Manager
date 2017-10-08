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

        public TaskContainerManager(Func<string, bool> CallBackExit)
        {
            CallBackFunction = CallBackExit;
        }

        private Func<string, bool> CallBackFunction = null;


        private class TaskItem
        {
            public String TaskName { get; set; }
            public int Id { get; set; }
            public Task Task_ { get; set; }
            private int HashCode { get; set; }

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
        /// <summary>
        ///   Wait All tasks from container
        /// </summary>
        public void WaitAll()
        {
            while (TasksContainer.Count > 0)
            {
                try
                {
                    List<Task> TaskList = new List<Task>();
                    foreach (var item in TasksContainer.Values)
                    {
                        TaskList.Add(item.Task_);
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
                    TaskItem item = TasksContainer.Values.FirstOrDefault(x => x.TaskName == name);
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
                    List<TaskItem> items = TasksContainer.Values.ToList<TaskItem>();
                    TaskItem item = items.FirstOrDefault(x => x.TaskName == TaskName);
                    if (item != null)
                    {
                        Task task = item.Task_;
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
                String Name = Remove(t1);
                if (CallBackFunction != null)
                {

                    CallBackFunction(Name);
                }
            });
            String _TaskName = task.Id.ToString();
            int Task_ID = task.Id;
            if (taskName != null)
            {
                _TaskName = taskName;
            }
            var taskItem = new TaskItem
            {
                TaskName = _TaskName,
                Id = task.Id,
                Task_ = task
            };



            List<TaskItem> items = TasksContainer.Values.ToList<TaskItem>();
            TaskItem item = items.FirstOrDefault(x => x.TaskName == _TaskName);

            if (!(item is null)) return false;

            bool ok = TasksContainer.TryAdd(taskItem.Id, taskItem);

            if (ok)
            {
                try
                {
                    task.Start();
                    return true;
                }
                catch
                {
                    Remove(task);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        ///    Remove task from container
        /// </summary>
        /// <param name="task"></param>
        /// <returns>TaskItem</returns>
        public String Remove(Task task)
        {
            TaskItem outItem = null;
            String TaskName = null;
            if (task == null) return null;
            try
            {

                TasksContainer.TryRemove(task.Id, out outItem);
                if (outItem != null)
                {
                    TaskName = outItem.TaskName;
                }
                task.Dispose();
            }
            catch (Exception ex)
            {

            }
            return TaskName;
        }
    }
}
