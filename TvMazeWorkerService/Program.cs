using TvMazeStorageService;
using TvMazeStorageService.Models;
using TvMazeWorkerService;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddHostedService<Worker>();
        services.Configure<TvMazeDatabaseSettings>(context.Configuration.GetSection("TvMazeDatabase"));
        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<IRestClient, RestClient>();

    });


IHost host = builder.Build();

await host.RunAsync();
