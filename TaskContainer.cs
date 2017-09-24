using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private ConcurrentDictionary<String,Task> TasksContainer = new ConcurrentDictionary<String, Task>();

        /// <summary>
        ///  Add a task to container
        /// </summary>
        /// <param name="task"></param>
        public void Add(Task task)
        {
            task.ContinueWith(t1 =>
            {
                Task rem;
                TasksContainer.Remove(t1.ToString(),out rem);
                t1.Dispose();
            });
            TasksContainer.TryAdd(task.ToString(),task);
            task.Start();
        }


    }
}
