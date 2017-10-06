using SGcombo.TaskContainer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleApp
{
    class Program
    {

        /// <summary>
        ///   Create Static Task Container
        /// </summary>
        public static TaskContainerManager taskContainer = new TaskContainerManager();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello TaskContainerManager!");
            for (int i = 1; i < 10; i++)
            {
                int ii = i;
                Task task = new Task(() => { PrintMessage(ii); });  // Create Task
                taskContainer.TryAdd(task);  // Add task to Container . Task will be start automatically
            }

            taskContainer.WaitAll();  // Wait all scheduled tasks

        }

        private static void PrintMessage(int j)
        {
            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine($"TaskContainerManager {j} > {i}");
                Thread.Sleep(1000);
            }
        }


    }
}
