using SGcombo.TaskContainer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleApp
{
    class Program
    {


        public static bool CallBack(String TaskName)
        {
            Console.WriteLine($"Task Completed : {TaskName} , TaskConteiner count {taskContainer2.Count()}");
            return true;
        }


        /// <summary>
        ///   Create Static Task Container
        /// </summary>
        public static TaskContainerManager taskContainer = new TaskContainerManager();
        public static TaskContainerManager taskContainer2 = new TaskContainerManager(CallBack);

        static void Main(string[] args)
        {
            Console.WriteLine("Hello TaskContainerManager!");
            for (int i = 1; i < 5; i++)
            {
                int ii = i;
                Task task = new Task(() => { PrintMessage(ii); });  // Create Task
                taskContainer.TryAdd(task);  // Add task to Container . Task will be start automatically
            }

            taskContainer.WaitAll();  // Wait all scheduled tasks


            Console.WriteLine("Hello TaskContainerManager test with callback !");
            for (int i = 1; i < 5; i++)
            {
                int ii = i;
                Task task = new Task(() => { PrintMessage(ii); });  // Create Task
                taskContainer2.TryAdd(task);  // Add task to Container . Task will be start automatically
            }

            taskContainer2.WaitAll();  // Wait all scheduled tasks



        }




        private static void PrintMessage(int j)
        {
            for (int i = 1; i < 10; i++)
            {
                Console.WriteLine($"TaskContainerManager {j} > {i}");
                Thread.Sleep(j*400);
            }
        }


    }
}
