using System;
using System.Threading;
using System.Threading.Tasks;
/*

Copyright (C) 2016-2018 by Vladimir Novick http://www.linkedin.com/in/vladimirnovick ,

    vlad.novick@gmail.com , http://www.sgcombo.com , https://github.com/Vladimir-Novick

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/
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
