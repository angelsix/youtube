using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TasksInConsole
{
    class Program
    {
        #region Private Members

        /// <summary>
        /// The event finished callback for the Thread event example
        /// </summary>
        private static event Action EventFinished = () => { };

        /// <summary>
        /// Whether to run the thread examples
        /// </summary>
        private static bool RunThreadExamples = false;

        #endregion

        static void Main(string[] args)
        {
            // Log it
            Log("Hello World!");

            //
            //   Author:         Luke Malpass
            //   License:        MIT
            //   Support Me:     https://www.patreon.com/angelsix
            //   Source Code:    http://www.github.com/angelsix/youtube/Tasks
            //   Website:        http://www.angelsix.com
            //   Contact:        contact@angelsix.com
            //
            //
            //    What is Asynchronous
            //   ======================
            //
            //     Asynchronous is if you start something, and don't wait while its happening. 
            //     It literally means to not occur at the same time.
            //
            //     This means not that our code returns early, but rather it doesn't sit there
            //     blocking the code while it waits (doesn't block the thread)
            //

            //
            //    Issues with Threads
            //   =====================
            //
            //     Threads are asynchronous, as they naturally do something while the calling thread 
            //     that made it doesn't wait for it.
            //

            #region Threads are asynchronous

            if (RunThreadExamples)
            {
                // Log it
                Log("Before first thread");

                // Start new thread
                new Thread(() =>
                {
                    // Sleep a little
                    Thread.Sleep(500);

                    // Log it
                    Log("Inside first thread");

                }).Start();

                // Log it
                Log("After first thread");

                // Wait for work to finish
                Thread.Sleep(1000);

                Console.WriteLine("---------------------------------------------");
            }

            #endregion

            //     What's the issue with Threads? 
            //     
            //      1. Expensive to make
            //      2. Not natural to be able to resume after a thread has finished to do something 
            //         related to the thread that created it
            //     
            //     Issue 1 was solved with a ThreadPool. However issue 2 is still an issue for threads, 
            //     and is one reason why Tasks were made.In order to resume work after some 
            //     asynchronous operation has occurred we could with a Thread:
            //     
            //      1. Block your code waiting for it (no better than just doing it on same thread)
            //

            #region Blocking Wait

            if (RunThreadExamples)
            {
                // Log it
                Log("Before blocking thread");

                // Create new thread
                var blockingThread = new Thread(() =>
                {
                    // Sleep a little
                    Thread.Sleep(500);

                    // Log it
                    Log("Inside blocking thread");
                });

                // Start thread
                blockingThread.Start();

                // Block and wait
                blockingThread.Join();

                // Log
                Log("After blocking thread");
                Console.WriteLine("---------------------------------------------");
            }

            #endregion

            //      2. Constantly poll for completion, waiting for a bool flag to say done (inefficient, slow)

            #region Polling Wait

            if (RunThreadExamples)
            {
                // Log it
                Log("Before polling thread");

                // Create poll flag
                var pollComplete = false;

                // Create thread
                var pollingThread = new Thread(() =>
                {
                    // Log it
                    Log("Inside polling thread");

                    // Sleep a little
                    Thread.Sleep(500);

                    // Set flag complete
                    pollComplete = true;
                });

                // Start thread
                pollingThread.Start();

                // Poll for completion
                while (!pollComplete)
                {
                    // Log it
                    Log("Polling....");

                    // Sleep a little
                    Thread.Sleep(100);
                }

                // Log it
                Log("After polling thread");
                Console.WriteLine("---------------------------------------------");
            }

            #endregion

            //      3. Event-based callbacks (lose the calling thread on callback, and causes nesting)

            #region Event-based Wait

            if (RunThreadExamples)
            {
                // Log it
                Log("Before event thread");

                // Create thread
                var eventThread = new Thread(() =>
                {
                    // Log it
                    Log("Inside event thread");

                    // Sleep a little
                    Thread.Sleep(500);

                    // Fire completed event
                    EventFinished();
                });

                // Hook into callback event
                EventFinished += () =>
                {
                    // Log it
                    Log("Event thread callback on complete");
                };

                // Start thread
                eventThread.Start();

                // Log it
                Log("After event thread");

                // Wait for work to finish
                Thread.Sleep(1000);

                Console.WriteLine("---------------------------------------------");
            }

            #endregion

            #region Event-based Wait Method

            if (RunThreadExamples)
            {
                // Log it
                Log("Before event method thread");

                // Call event callback style method
                EventThreadCallbackMethod(() =>
                {
                    // Log it
                    Log("Event thread callback on complete");
                });

                // Log it
                Log("After event method thread");

                // Wait for work to finish
                Thread.Sleep(1000);

                Console.WriteLine("---------------------------------------------");
            }

            #endregion

            //     
            //     However that makes every time we want to do something asynchronous a lot of code
            //     and not easy to follow.
            //

            //
            //    What is a Task
            //   ================
            //
            //     A Task encapsulates the promise of an operation completing in the future
            //

            //
            //    Tasks, Async and Await
            //   ========================
            //
            //     Async in C# is mainly 2 words. async and await
            //
            //     The point is to allow easy and clean asynchronous code to be written without complex or messy code.
            //

            #region Sync vs Async Method

            // Log it
            Log("Before sync thread");

            // Website to fetch
            var website = "http://www.google.co.uk";

            // Download the string
            WebDownloadString(website);

            // Log it
            Log("After sync thread");

            Console.WriteLine("---------------------------------------------");

            // Log it
            Log("Before async thread");

            // Download the string asynchronously
            var downloadTask = WebDownloadStringAsync(website);

            // Log it
            Log("After async thread");

            // Wait for task to complete
            downloadTask.Wait();

            Console.WriteLine("---------------------------------------------");

            var task = Task.Run(async () =>
            {
                // Log it
                Log("Before async await thread");

                // Download the string asynchronously
                await WebDownloadStringAsync(website);

                // Log it
                Log("After async await thread");

                Console.WriteLine("---------------------------------------------");
            });

            // Wait the main task
            task.Wait();

            #endregion

            //     Async and await are always used together. A method or lambda tagged with 
            //     async can then await any Task
            //
            //     When you await something, the thread which called the await is free to then return to 
            //     what it was doing, while in parallel the task inside the await is now run on another thread. 
            //     
            //     Once the task is done, it returns either to the original calling thread, or carries on, 
            //     on another thread to do the work that codes after the await.
            //
            //
            //
            //    Async Analogy
            //   ===============
            //
            //     Imagine you go to Starbucks and the entire shop is run by one person.
            //     His name is Mr UI Thread. You walk in and ask Mr Thread for a Vanilla Latte. 
            //     He obliges and starts to make your coffee. 
            //
            //     He puts the milk into the container and turns on the hot steam, and proceeds 
            //     to stand there and wait for the milk to reach 70 degrees.
            //
            //     During this time you remember you wanted a muffin as well, so you shout over
            //     to Mr Thread and ask for a muffin... but he ignores you. He is blocked 
            //     waiting for the milk to boil.
            //     
            //     Several minutes goes by and 3 more customers have come in and are waiting 
            //     to be served. Finally the milk is finished and he completes the Latte. 
            //     Returning to you. You are a little annoyed at being ignored for minutes
            //     and decide to leave your muffin. 
            //
            //     Then he continues to serve one customer at a time, doing one job at a time.
            //     Not a good situation.
            //     
            //     This is what happens with a single threaded application.
            //     
            //     Now in order to improve business Mr Thread employs 2 new members of staff
            //     called Mrs and Mrs Worker Thread. The pair work well independently and 
            //     as Mr Thread takes orders from the customers, he asks Mrs Worker Thread 
            //     to complete the order, and then without waiting for Mrs Worker Thread to
            //     finish the drink, proceeds to serve the next customer.
            //     
            //     Once Mrs Worker Thread has finished a drink, instead of having to take 
            //     the drinks to the customers she asks Mr Worker Thread to serve the drinks 
            //     and then without waiting she proceeds to start the next order.
            //     
            //     The business is now a well-oiled, multi-threaded business.
            //

            //
            //    The Synchronous part in Tasks
            //   ===============================
            //
            //

            #region The Synchronous Part of Tasks

            // Run some work to show the synchronous parts of the call
            Task.Run(async () =>
            {
                // Log it
                Log($"Before DoWork thread");

                // Do work
                // This will return a Task and run the lines of code inside the method
                // up until the point at which the first await is hit
                var doWorkTask = DoWorkAsync("me");

                // Await the task
                // This will then spin off to a new thread and come with the result
                await doWorkTask;

                // Log it
                Log($"After DoWork thread");

            }).Wait();

            Console.WriteLine("---------------------------------------------");

            #endregion

            //
            //    Async Return Types
            //   ====================
            //
            //     You can only return void, Task or Task<T> for a method marked as async, as the method is 
            //     not complete when it returns, so no other result is valid.
            //

            #region Method 1 Getting Result of Async From Sync

            // Get the task
            var doWorkResultTask = DoWorkAndGetResultAsync("Return this");

            // Wait for it
            doWorkResultTask.Wait();

            // Get the result
            var doWorkResult = doWorkResultTask.Result;

            Console.WriteLine("---------------------------------------------");

            #endregion

            #region Method 2 Getting Result of Async From Sync

            Task.Run(async () =>
            {
                var doWorkResult2 = await DoWorkAndGetResultAsync("Return this 2");

            }).Wait();

            Console.WriteLine("---------------------------------------------");

            #endregion

            //
            //    Async Keyword
            //   ===============
            //
            //     The async keyword is not actually added to the method declaration signature, 
            //     the only effect is has is to change the compiled code. 
            //
            //     That's why interfaces cannot declare async, as it isn't a declarative statement, 
            //     its a compilation statement to change the flow of the function.
            //

            //
            //    Consuming Async Methods
            //   =========================
            //
            //     The best way to consume or call a method that returns a task is to be async yourself
            //     in the caller method, to ultimately awaiting it. 
            //
            //     By that definition async methods are naturally contagious.
            //

            #region Consuming Wait

            // Store the taks 
            var workResultTask = DoWorkAndGetResultAsync("Consume Wait");

            // Wait for it 
            workResultTask.Wait();

            // Get the result
            var workResult = workResultTask.Result;

            Console.WriteLine("---------------------------------------------");

            #endregion

            #region Consuming via Task

            // Declare the result
            var workResultViaTask = default(string);

            // Store the taks 
            Task.Run(async () =>
            {
                // Get result
                workResultViaTask = await DoWorkAndGetResultAsync("Consume via Task");

            }).Wait();

            Console.WriteLine("---------------------------------------------");

            #endregion

            //
            //    What happens during an Async call
            //   ===================================
            //
            //     Code inside a function that returns a Task runs its code on the callers thread 
            //     up until the first line that calls await. At this point in time:
            //
            //      1. The current thread executing your code is released (making your code asynchronous).
            //         This means from a normal point of view, your function has returned at this point
            //         (it has return the Task object).
            //
            //      2. Once the task you have awaited completes, your method should appear to continue
            //         from where it left off, as if it never returned from the method, so resume on 
            //         the line below the await.
            //     
            //     To achieve this, C# at the point of reaching the await call:
            //     
            //      1. Stores all local variables in the scope of the method 
            //      2. Stores the parameters of your method
            //      3. The "this" variable to store all class-level variables
            //      4. Stores all contexts(Execution, Security, Call)
            //     
            //     And on resuming to the line after the await, restores all of these values as if nothing
            //     had changed. All of this information is stored on the garbage collection heap.
            //

            //
            //    What is happening with threads during an async call
            //   =====================================================
            //
            //     As you call a method that returns a `Task` and uses `async`, inside the method all code, 
            //     up until the first `await` statement, is run like a normal function on the thread that 
            //     called it.
            //
            //     Once it hits the `await` the function returns the `Task` to the caller, and does its work 
            //     thats inside the `await` call on a new thread (or existing).
            //
            //     Once its done, and effectively "after" the `await` line, execution returns to a certain 
            //     thread.
            //     
            //     That thread is determined by first checking if the thread has an synchronization context 
            //     and if it does it asks that what thread to return to. For UI threads this will return work
            //     to the UI thread itself. 
            //

            // Console application has no synchronization context
            var syncContext = SynchronizationContext.Current;

            //
            //     For normal threads that have no synchronization context, the code after the `await` 
            //     typically, but not always, continues on the same thread that the inner work was being done 
            //     on, but has no requirement to resume on any specific thread.
            //     
            //     Typically if you use `ContinueWith` instead of `await`, the code inside `ContinueWith` runs 
            //     on a different thread than the inner task was running on, and using `await` typically  
            //     continues on the same thread.
            //     

            // Show ContinueWith typically changing thread ID's
            DoWorkAsync("ContinueWith").ContinueWith(t =>
            {
                Log("ContinueWith Complete");
            }).Wait();

            Console.WriteLine("---------------------------------------------");

            //
            //     This also means after every await the next line is typically on a new thread if there is no 
            //     synchronization context.
            //
            //    **********************************************************************************************
            // 
            //      An exception is if you use `ConfigureAwait(false)` then the SynchronizationContext is 
            //      totally  ignored and the resuming thread is treated as if there were no context.
            //
            //      Resuming on the original thread via the synchronization context is an expensive thing 
            //      (takes time) and so if you choose to not care about resuming on the same thread and want
            //      to save time you can use `ConfigureAwait` to remove that overhead
            //
            //    **********************************************************************************************
            //

            //
            //    Exceptions in Async calls
            //   ===========================
            //
            //     Any exceptions thrown that are not caught by the method itself are thrown into the Task 
            //     objects value `IsFaulted` and the `Exception` property.
            //
            //     If you do not await the Task, the exception will not throw on your calling thread.
            //

            #region Throw on Calling Thread, Without Awaiting

            Log("Before ThrowAwait");

            var crashedTask = ThrowAwait(true);

            // Did it crash?
            var isFaulted = crashedTask.IsFaulted;

            // The exception
            Log(crashedTask.Exception.Message);

            Log("After ThrowAwait");

            #endregion

            //
            //     If you await, the exception will rethrow onto the caller thread that awaited it.
            //     
            //     The exception to the rule is a method with async void. As it cannot be awaited, any
            //     exceptions that occur in an async void method are re-thrown like this:
            //     
            //      1. If there is a synchronization context the exception is Post back to the caller thread. 
            //      2. If not, it is thrown on the thread pool
            //

            #region Throw on Calling Thread, Without Awaiting

            Log("Before ThrowVoid");

            ThrowAwaitVoid(true);

            Log("After ThrowVoid");

            #endregion

            Console.ReadLine();
        }

        #region Helper Methods

        /// <summary>
        /// Output a message with the current thread ID appended
        /// </summary>
        /// <param name="message"></param>
        private static void Log(string message)
        {
            // Write line
            Console.WriteLine($"{message} [{Thread.CurrentThread.ManagedThreadId}]");
        }

        #endregion

        #region Thread Methods

        /// <summary>
        /// Shows an event-based thread callback via a method
        /// </summary>
        /// <param name="completed">The callback to call once the work is complete</param>
        private static void EventThreadCallbackMethod(Action completed)
        {
            // Start a new thread
            new Thread(() =>
            {
                // Log it
                Log("Inside event thread method");

                // Sleep
                Thread.Sleep(500);

                // Fire completed event
                completed();

            }).Start();
        }

        #endregion

        #region Task Example Methods

        /// <summary>
        /// Downloads a string from a website URL sychronously
        /// </summary>
        /// <param name="url">The URL to download</param>
        private static void WebDownloadString(string url)
        {
            // Synchronous pattern
            var webClient = new WebClient();
            var result = webClient.DownloadString(url);

            // Log
            Log($"Downloaded {url}. {result.Substring(0, 10)}");
        }

        /// <summary>
        /// Downloads a string from a website URL asychronously
        /// </summary>
        /// <param name="url">The URL to download</param>
        private static async Task WebDownloadStringAsync(string url)
        {
            // Asynchronous pattern
            var webClient = new WebClient();
            var result = await webClient.DownloadStringTaskAsync(new Uri(url));

            // Log
            Log($"Downloaded {url}. {result.Substring(0, 10)}");
        }

        /// <summary>
        /// Does some work asynchronously for somebody
        /// </summary>
        /// <param name="forWho">Who we are doing the work for</param>
        /// <returns></returns>
        private static async Task DoWorkAsync(string forWho)
        {
            // Log it
            Log($"Doing work for {forWho}");

            // Start a new task (so it runs on a different thread)
            await Task.Run(async () =>
            {
                // Log it
                Log($"Doing work on inner thread for {forWho}");

                // Wait 
                await Task.Delay(500);

                // Log it
                Log($"Done work on inner thread for {forWho}");
            });

            // Log it
            Log($"Done work for {forWho}");
        }

        /// <summary>
        /// Does some work asynchronously for somebody, and return a result
        /// </summary>
        /// <param name="forWho">Who we are doing the work for</param>
        /// <returns></returns>
        private static async Task<string> DoWorkAndGetResultAsync(string forWho)
        {
            // Log it
            Log($"Doing work for {forWho}");

            // Start a new task (so it runs on a different thread)
            await Task.Run(async () =>
            {
                // Log it
                Log($"Doing work on inner thread for {forWho}");

                // Wait 
                await Task.Delay(500);

                // Log it
                Log($"Done work on inner thread for {forWho}");
            });

            // Log it
            Log($"Done work for {forWho}");

            // Return what we received
            return forWho;
        }

        /// <summary>
        /// Throws an exception inside a task
        /// </summary>
        /// <param name="before">Throws the exception before an await</param>
        /// <returns></returns>
        private static async Task ThrowAwait(bool before)
        {
            if (before)
                throw new ArgumentException("Oopps");

            await Task.Delay(1);

            throw new ArgumentException("Oopps");
        }

        /// <summary>
        /// Throws an exception inside a void async before awaiting
        /// </summary>
        /// <param name="before">Throws the exception before an await</param>
        /// <returns></returns>
        private static async void ThrowAwaitVoid(bool before)
        {
            if (before)
                throw new ArgumentException("Oopps");

            await Task.Delay(1);

            throw new ArgumentException("Oopps");
        }
        #endregion
    }
}
