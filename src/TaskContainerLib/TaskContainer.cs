using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime;
using System.Reflection;

////////////////////////////////////////////////////////////////////////////
//	Copyright 2016 - 2017 : Vladimir Novick    https://www.linkedin.com/in/vladimirnovick/  
//
//    NO WARRANTIES ARE EXTENDED. USE AT YOUR OWN RISK. 
//
// To contact the author with suggestions or comments, use  :vlad.novick@gmail.com
//
////////////////////////////////////////////////////////////////////////////
namespace TaskContainerLib
{
    

    /// <summary>
    ///    Running multiple tasks asynchronously
    /// </summary>
    public class TaskContainerManager
    {

        

        /// <summary>
        ///   Specify Task Manager option. Multiple Conditions with  || TaskContainerManager Options
        /// </summary>
        [Flags] // indicates bitwise operations occur on this enum
        public enum Options : byte
        {
            None = 0,
            LargeObjectHeapCompactionMode =  1,
            GCCollect =  2
        }

        public TaskContainerManager()
        {
            OnTaskExit = null;
        }

        /// <summary>
        ///   Add Task Manager option. Multiple Conditions with  || TaskContainerManager Options
        /// </summary>
        public Options Option { private get; set; }
        

        /// <summary>
        ///    Add Task OnExit Function
        /// </summary>
        /// <param name="CallBackExit"></param>
        public Func<string, bool> OnTaskExit { private get; set; }





  
        /// <summary>
        ///   Wait All tasks from container
        /// </summary>
        public void WaitAll()
        {
            while (TasksContainer.Count > 0)
            {
                WaitAny();
            }

        }
        /// <summary>
        ///   Wait all task by specifications list
        /// </summary>
        /// <param name="taskNames"></param>
        public void WaitAll(List<String> taskNames)
        {
            try
            {

                List<Task> TaskList = new List<Task>();
                foreach (TaskItem item in TasksContainer.Values)
                {
                    try
                    {
                        var ok = taskNames.Find(m => m == item.TaskName);
                        if (ok != null)
                        {
                            if (item.Task_ != null)
                            {
                                TaskList.Add(item.Task_);
                            }
                        }
                    }
                    catch (Exception) { }
                }

                Task.WaitAll(TaskList.ToArray());
            }
            catch { }
        }

        /// <summary>
        ///    Set Current Task Status 
        /// </summary>
        /// <param name="taskName"></param>
        /// <param name="CurrentStatus"></param>
        public void SetCurrentStatus(string taskName, String CurrentStatus)
        {
            if (TasksContainer.Count > 0)
            {
                TaskItem item = TasksContainer.Values.FirstOrDefault(x => x.TaskName == taskName);
                if (item != null)
                {
                    item.CurrentStatus = CurrentStatus;
                }
            }
        }

        /// <summary>
        ///    Get current task status string
        /// </summary>
        /// <param name="taskName"></param>
        public string GetCurrentStatus(string taskName)
        {
          
            if (TasksContainer.Count > 0)
            {
                TaskItem item = TasksContainer.Values.FirstOrDefault(x => x.TaskName == taskName);
                if (item != null)
                {
                   return  item.CurrentStatus ;
                }
            }
            return "Finished";
        }


        /// <summary>
        ///    Get all active task
        /// </summary>
        /// <returns></returns>
        public List<TaskItemStatus> GetStatuses()
        {
            List<TaskItemStatus> TaskList = new List<TaskItemStatus>();
            try
            {
              
                foreach (var item in TasksContainer.Values)
                {
                    try
                    {
                        TaskItemStatus itemStatus = new TaskItemStatus()
                        {
                            Name = item.TaskName,
                            Description = item.Description,
                            Start = item.StartTime
                           
                        };
                        if (item.Task_ == null)
                        {
                            itemStatus.Status = TaskStatus.Canceled;
                        }
                        else
                        {
                            try
                            {
                                itemStatus.Status = item.Task_.Status;
                            } catch ( Exception)
                            {
                                itemStatus.Status = TaskStatus.Canceled;
                            }
                        }

                        TaskList.Add(itemStatus);
                    }
                    catch (Exception) { }
                }

              
            }
            catch { }
            return TaskList;
        }


        /// <summary>
        ///   Wait Any Task
        /// </summary>
        public void WaitAny()
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
        /// <param name="description"></param>
        /// <param name="callBack">   bool myCallBack(string taskName ) </param>
        /// <returns></returns>
        public bool TryAdd(Task task, String taskName = null, String description = null , Func<String,bool> callBack = null)
        {
            task.ContinueWith(t1 =>
            {

                 TaskItem outTaskItem = null;
               if ( TasksContainer.TryGetValue(task.Id, out outTaskItem))
                {
                    if (outTaskItem.Callback != null)
                    {
                        try
                        {
                            outTaskItem.Callback(outTaskItem.TaskName);
                        }
                        catch ( Exception ) { }
                    }


                }


                String Name = Remove(t1);

                if ((TaskContainerManager.Options.LargeObjectHeapCompactionMode & this.Option) == TaskContainerManager.Options.LargeObjectHeapCompactionMode)
                {
                    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
                }


                if ((TaskContainerManager.Options.GCCollect & this.Option) == TaskContainerManager.Options.GCCollect)
                {
                    GC.Collect();
                }

                if (OnTaskExit != null)
                {

                    OnTaskExit(Name);
                }
                t1.Dispose();
            });
            String _TaskName = $"Task: {task.Id}";

            String _description = description;


            int Task_ID = task.Id;
            if (taskName != null)
            {
                _TaskName = taskName;
            } 
            if (_description == null)
            {
                try
                {
                    var fieldInfo = typeof(Task).GetField("m_action", BindingFlags.Instance | BindingFlags.NonPublic);
                    Delegate action = fieldInfo.GetValue(task) as Delegate;
                    if ( action != null )
                    {
                        var name = action.Method.Name;
                        var type = action.Method.DeclaringType.FullName;
                        _description = $"Method: {name} Type: {type}";
                    
                        
                    } 
                }
                catch (Exception) { }
            }
            var taskItem = new TaskItem
            {
                TaskName = _TaskName,
                Id = task.Id,
                Task_ = task,
                StartTime = DateTime.Now,
                Description = _description,
                Callback = callBack,
                CurrentStatus = "Started"
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
            catch (Exception )
            {

            }
            return TaskName;
        }
    }
}
