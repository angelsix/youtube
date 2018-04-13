using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TasksInWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Private Members

        /// <summary>
        /// Whether to do synchronous work on UI thread or not
        /// </summary>
        private bool mDoSynchronousWork = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        #endregion

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            #region Doing synchronous work on UI thread

            if (mDoSynchronousWork)
            {
                Log("Start synchronous work");

                Thread.Sleep(5000);

                Log("End synchronous work");
            }

            #endregion

            #region Doing Async work on UI thread

            // Log it
            Log("Start asynchronous work");

            // Remember UI context
            // var uiSyncContext = SynchronizationContext.Current;

            // To kill UI sync context on capture in await
            // SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());

            // Run some async work
            await DoWorkAsync("Async"); //.ConfigureAwait(false);

            // Should resume on same UI thread, thanks to SynchronizationContext

            // Set UI stuff
            // uiSyncContext.Post((s) =>
            // {
            //      MyButton.Content = "New content";
            // }, null);

            // MyButton.Content = "New content";

            // Log it
            Log("End asynchronous work");

            #endregion
        }

        #region Helper Methods

        /// <summary>
        /// Output a message with the current thread ID appended
        /// </summary>
        /// <param name="message"></param>
        private static void Log(string message)
        {
            // Write line
            Debug.WriteLine($"{message} [{Thread.CurrentThread.ManagedThreadId}]");
        }

        #endregion


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

    }
}
