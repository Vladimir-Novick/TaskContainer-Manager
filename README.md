# TaskContainer-Manager
Start multiple asynchronous tasks .

By using TaskContainerManager, you can start multiple tasks at the different time and process them.  TaskContainerManager is a .NET Core library. 

You must be including TaskContainerManager in an existing .NET Core application without installation additional components

Example:

 ### Example:

####   Create callback functions:

1) Global callback function:
       A callback function is executed after any task is finished
       
        public static bool OnTaskExiFunctiont(String TaskName)
        {
            Console.WriteLine($"Task Completed : {TaskName} ");
            return true;
        }

2) Task exit callback function:
      A callback function is executed if specific task finished.

        public static bool CallBackFunction(String taskName)
        {
            Console.WriteLine($" *****  CallBack : {taskName}");
            return true;
        }


####   Using:

            TaskContainerManager taskContainer = new TaskContainerManager();
            taskContainer.OnTaskExit = OnTaskExiFunctiont;
            taskContainer.Option = TaskContainerManager.Options.None;

            for (int i = 1; i < 5; i++)
            {
                int ii = i;
                string taskName = $"Task {ii}";
                Task task = new Task(() => { PrintMessage(ii, taskName); });  // Create Task
		            // Add task to Container . Task will be start automatically
                taskContainer.TryAdd(task, taskName, $"Task Description {i}", CallBackFunction); 
            }
            taskContainer.WaitAll();  // Wait all scheduled tasks

	

Copyright (C) 2016-2018 by Vladimir Novick http://www.linkedin.com/in/vladimirnovick , 

vlad.novick@gmail.com , http://www.sgcombo.com , https://github.com/Vladimir-Novick	

# License

		Copyright (C) 2016-2018 by Vladimir Novick http://www.linkedin.com/in/vladimirnovick , 

		Permission is hereby granted, free of charge, to any person obtaining a copy
		of this software and associated documentation files (the "Software"), to deal
		in the Software without restriction, including without limitation the rights
		to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
		copies of the Software, and to permit persons to whom the Software is
		furnished to do so, subject to the following conditions:

		The above copyright notice and this permission notice shall be included in
		all copies or substantial portions of the Software.

		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
		IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
		FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
		AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
		LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
		OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
		THE SOFTWARE. 



