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
            System.Diagnostics.Debug.WriteLine($"Task Completed : {TaskName} , Task Conteiner count {taskContainer.Count()}");
            return true;
        }


        public void WaitAllTask()
        {
            while (taskContainer.Count() > 0)
            {
                System.Diagnostics.Debug.WriteLine($" Task Completed ");
                taskContainer.WaitAny();
            }

        }


        private static void PrintMessage(int j)
        {
            for (int i = 1; i < 10; i++)
            {
                System.Diagnostics.Debug.WriteLine($"TaskContainerManager {j} > {i}");
                Thread.Sleep(j * 400);
            }
        }

        [TestMethod]
        public void MultiTask()
        {

            taskContainer.OnTaskExit = null;
            taskContainer.Option = TaskContainerManager.Options.None;

            for (int i = 1; i < 5; i++)
            {
                int ii = i;
                Task task = new Task(() => { PrintMessage(ii); });  // Create Task
                taskContainer.TryAdd(task);  // Add task to Container . Task will be start automatically
            }
            WaitAllTask();  // Wait all scheduled tasks
        }


        [TestMethod]
        public void OnTaskExit()
        {

            taskContainer.OnTaskExit = OnTaskExiFunctiont;
            taskContainer.Option = TaskContainerManager.Options.None;

            for (int i = 1; i < 5; i++)
            {
                int ii = i;
                Task task = new Task(() => { PrintMessage(ii); });  // Create Task
                taskContainer.TryAdd(task);  // Add task to Container . Task will be start automatically
            }
            WaitAllTask();  // Wait all scheduled tasks
        }

        [TestMethod]
        public void AddOptions()
        {
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
    }
}
