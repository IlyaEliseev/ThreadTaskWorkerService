using ThreadTaskWorkerService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<ThreadTaskSyncWorker>();
    })
    .Build();

await host.RunAsync();
