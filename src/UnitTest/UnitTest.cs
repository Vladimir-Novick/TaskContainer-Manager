using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskContainerLib;

namespace UnitTest
{
    [TestClass]
    public class UnitTest
    {

        public static TaskContainerManager taskContainer = new TaskContainerManager();

        public static bool OnTaskExiFunctiont(String TaskName)
        {
            Console.WriteLine($"Task Completed : {TaskName} , Task Container. Count: {taskContainer.Count()}");
            return true;
        }


        public static bool OnTaskExiFunctiont2(String TaskName)
        {
            var statuses = taskContainer.GetStatuses();
            foreach (var p in statuses)
            {
                Console.WriteLine($" task: {p.Name}, start time: {p.Start} , status: {p.Status}");
            }
            return true;
        }

        public void WaitAllTask()
        {
            while (taskContainer.Count() > 0)
            {
                Console.WriteLine($" Task Completed ");
                taskContainer.WaitAny();
            }

        }


        private static void PrintMessage(int j)
        {
            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine($"Task {j} > {i}");
                Thread.Sleep(j * 400);
            }
        }

        private static void PrintMessage(int j, string taskName)
        {
            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine($"{taskName} > {i}");
                taskContainer.SetCurrentStatus(taskName, $"inprogress {i}");
                String currentStatus = taskContainer.GetCurrentStatus(taskName);
                Console.WriteLine($"Task {taskName},  Current status > {currentStatus}");
                Thread.Sleep(j * 400);
            }
        }

        [TestMethod]
        public void MultiTask()
        {
            Console.WriteLine("--------------------MultiTask --------------------");
            taskContainer.OnTaskExit = null;
            taskContainer.Option = TaskContainerManager.Options.None;

            for (int i = 1; i < 5; i++)
            {
                int ii = i;
                Task task = new Task(() => { PrintMessage(ii); });  // Create Task
                taskContainer.TryAdd(task);  // Add task to Container . Task will be start automatically
            }
            taskContainer.WaitAll();  // Wait all scheduled tasks
        }


        public static bool CallBackFunction(String taskName)
        {
            Console.WriteLine($" *****  CallBack : {taskName}");
            Console.WriteLine($" ****** Task Counter : {taskContainer.Count()}");
            return true;
        }


        [TestMethod]
        public void CallBackFunction()
        {
            Console.WriteLine("--------------------CallBackFunction --------------------");
            taskContainer.OnTaskExit = null;
            taskContainer.Option = TaskContainerManager.Options.None;

            for (int i = 1; i < 5; i++)
            {
                int ii = i;
                Task task = new Task(() => { PrintMessage(ii); });  // Create Task
                taskContainer.TryAdd(task,$"Task {i}",$"Task Description {i}", CallBackFunction);  // Add task to Container . Task will be start automatically
            }
            taskContainer.WaitAll();  // Wait all scheduled tasks
        }

        [TestMethod]
        public void SetCurrentStatus()
        {
            Console.WriteLine("--------------------SetCurrentStatus --------------------");
            taskContainer.OnTaskExit = null;
            taskContainer.Option = TaskContainerManager.Options.None;

            for (int i = 1; i < 5; i++)
            {
                int ii = i;
                string taskName = $"Task {ii}";
                Task task = new Task(() => { PrintMessage(ii, taskName); });  // Create Task
                taskContainer.TryAdd(task, taskName, $"Task Description {i}", CallBackFunction);  // Add task to Container . Task will be start automatically
            }
            taskContainer.WaitAll();  // Wait all scheduled tasks
        }



        [TestMethod]
        public void TaskTimeout()
        {
            Console.WriteLine("--------------------TaskTimeout --------------------");
            taskContainer.OnTaskExit = null;
            taskContainer.Option = TaskContainerManager.Options.None;

            {
                string taskName = $"Task 20 sec ";
                Task task = new Task(() => { PrintMessage(1000, taskName); });  // Create Task
                taskContainer.TryAdd(task, taskName, $"Task Description ", CallBackFunction, 40000);  // Add task to Container . Task will be start automatically
            }

            {
                string taskName = $"Task 20 sec 2 ";
                Task task = new Task(() => { PrintMessage(1000, taskName); });  // Create Task
                taskContainer.TryAdd(task, taskName, $"Task Description ", CallBackFunction, 40000);  // Add task to Container . Task will be start automatically
            }
            {
                string taskName = $"Task 5 sec ";
                Task task = new Task(() => { PrintMessage(1000, taskName); });  // Create Task
                taskContainer.TryAdd(task, taskName, $"Task Description ", CallBackFunction, 2000 );  // Add task to Container . Task will be start automatically
            }
            Thread.Sleep(8000);
            Console.WriteLine($" Count {taskContainer.Count()}");

            taskContainer.WaitAll();  // Wait all scheduled tasks
        }


        [TestMethod]
        public void OnTaskExit()
        {
            Console.WriteLine("--------------------OnTaskExit --------------------");
            taskContainer.OnTaskExit = OnTaskExiFunctiont;
            taskContainer.Option = TaskContainerManager.Options.None;

            for (int i = 1; i < 5; i++)
            {
                int ii = i;
                Task task = new Task(() => { PrintMessage(ii); });  // Create Task
                taskContainer.TryAdd(task);  // Add task to Container . Task will be start automatically
            }
            taskContainer.WaitAll(); // Wait all scheduled tasks
        }

        [TestMethod]
        public void AddOptions()
        {
            Console.WriteLine("--------------------AddOptions --------------------");
            taskContainer.OnTaskExit = null;
            taskContainer.Option = TaskContainerManager.Options.LargeObjectHeapCompactionMode | TaskContainerManager.Options.GCCollect;

            for (int i = 1; i < 5; i++)
            {
                int ii = i;
                Task task = new Task(() => { PrintMessage(ii); });  // Create Task
                taskContainer.TryAdd(task);  // Add task to Container . Task will be start automatically
            }
            taskContainer.WaitAll();  // Wait all scheduled tasks
        }


        [TestMethod]
        public void OnTaskStatuses()
        {
            Console.WriteLine("--------------------OnTaskStatuses --------------------");
            taskContainer.OnTaskExit = OnTaskExiFunctiont2;
            taskContainer.Option = TaskContainerManager.Options.None;

            for (int i = 1; i < 5; i++)
            {
                int ii = i;
                Task task = new Task(() => { PrintMessage(ii); });  // Create Task
                taskContainer.TryAdd(task);  // Add task to Container . Task will be start automatically
            }
            taskContainer.WaitAll();  // Wait all scheduled tasks
        }

    }
}
