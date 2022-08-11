namespace ThreadTaskWorkerService
{
    public class ThreadTaskSyncWorker : BackgroundService
    {
        private readonly ILogger<ThreadTaskSyncWorker> _logger;
        private int _count = 0;
        private readonly Mutex _mutex = new Mutex();
        private readonly Semaphore _semaphore = new Semaphore(1,3);
        private readonly AutoResetEvent _are = new AutoResetEvent(true);

        public ThreadTaskSyncWorker(ILogger<ThreadTaskSyncWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Clear files before start
            File.WriteAllText("ThreadMutex.txt", string.Empty);
            File.WriteAllText("ThreadSemaphore.txt", string.Empty);
            File.WriteAllText("ThreadAutoResetEvent.txt", string.Empty);
            File.WriteAllText("TaskMutex.txt", string.Empty);
            File.WriteAllText("TaskSemaphore.txt", string.Empty);
            File.WriteAllText("TaskAutoResetEvent.txt", string.Empty);

            // Thread sleep after threads because _count concurrent resources
            // Threads
            ThreadMutex();
            Thread.Sleep(1000);

            ThreadSemaphore();
            Thread.Sleep(1000);

            ThreadAutoResetEvent();
            Thread.Sleep(1000);

            // Tasks
            TaskMutex();
            Thread.Sleep(1000);

            TaskSemaphore();
            Thread.Sleep(1000);

            TaskAutoResetEvent();
            Thread.Sleep(1000);

            _mutex.Dispose();
            _semaphore.Dispose();
            _are.Dispose();

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }

        private void ThreadMutex()
        {
            for (int i = 0; i < 5; i++)
            {
                var threadMutex = new Thread(PrintContentSyncMutex);
                threadMutex.Name = $"ThreadMutex_{i + 1}";
                threadMutex.Start();
            }
        }

        private void ThreadSemaphore()
        {
            for (int i = 0; i < 5; i++)
            {
                var threadSemaphore = new Thread(PrintContentSyncSemaphore);
                threadSemaphore.Name = $"ThreadSemaphore_{i + 1}";
                threadSemaphore.Start();
            }
        }

        private void ThreadAutoResetEvent()
        {
            for (int i = 0; i < 5; i++)
            {
                var threadAutoResetEvent = new Thread(PrintContentSyncAutoResetEvent);
                threadAutoResetEvent.Name = $"ThreadAutoResetEvent_{i + 1}";
                threadAutoResetEvent.Start();
            }
        }

        private void TaskMutex()
        {
            for (int i = 0; i < 5; i++)
            {
                var taskMutex = Task.Run(PrintContentTaskSyncMutex);
            }
        }

        private void TaskSemaphore()
        {
            for (int i = 0; i < 5; i++)
            {
                var taskMutex = Task.Run(PrintContentTaskSyncSemaphore);
            }
        }

        private void TaskAutoResetEvent()
        {
            for (int i = 0; i < 5; i++)
            {
                var taskMutex = Task.Run(PrintContentTaskSyncAutoResetEvent);
            }
        }


        // Thread methods
        private void PrintContentSyncMutex()
        {
            _mutex.WaitOne();

            _count = 1;
            for (int i = 0; i < 5; i++)
            {
                File.AppendAllText("ThreadMutex.txt", $"{Thread.CurrentThread.Name}: {Guid.NewGuid()}_{_count}\n");
                _count++;
            }

            _mutex.ReleaseMutex();
        }
        
        private void PrintContentSyncSemaphore()
        {
            _semaphore.WaitOne();

            _count = 1;
            for (int i = 0; i < 5; i++)
            {
                File.AppendAllText("ThreadSemaphore.txt", $"{Thread.CurrentThread.Name}: {Guid.NewGuid()}_{_count}\n");
                _count++;
            }

            _semaphore.Release();
        }
        
        private void PrintContentSyncAutoResetEvent()
        {
            _are.WaitOne();

            _count = 1;
            for (int i = 0; i < 5; i++)
            {
                File.AppendAllText("ThreadAutoResetEvent.txt", $"{Thread.CurrentThread.Name}: {Guid.NewGuid()}_{_count}\n");
                _count++;
            }

            _are.Set();
        }

        // Task methods
        private void PrintContentTaskSyncMutex()
        {
            _mutex.WaitOne();

            _count = 1;
            for (int i = 0; i < 5; i++)
            {
                File.AppendAllText("TaskMutex.txt", $"{Task.CurrentId}: {Guid.NewGuid()}_{_count}\n");
                _count++;
            }

            _mutex.ReleaseMutex();
        }
        
        private void PrintContentTaskSyncSemaphore()
        {
            _semaphore.WaitOne();

            _count = 1;
            for (int i = 0; i < 5; i++)
            {
                File.AppendAllText("TaskSemaphore.txt", $"{Task.CurrentId}: {Guid.NewGuid()}_{_count}\n");
                _count++;
            }

            _semaphore.Release();
        }
        
        private void PrintContentTaskSyncAutoResetEvent()
        {
            _are.WaitOne();

            _count = 1;
            for (int i = 0; i < 5; i++)
            {
                File.AppendAllText("TaskAutoResetEvent.txt", $"{Task.CurrentId}: {Guid.NewGuid()}_{_count}\n");
                _count++;
            }

            _are.Set();
        }
    }
}