using MongoDB.Bson;
using TvMazeStorageService;
using TvMazeStorageService.Models;

namespace TvMazeWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStorageService _storageService;
        private readonly IRestClient _restClient;

        public Worker(ILogger<Worker> logger, IStorageService storageService, IRestClient restClient)
        {
            _logger = logger;
            _storageService = storageService;
            _restClient = restClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                int latestShowId = (await _storageService.GetMostRecentShowAsync())?.id ?? 0;

                await foreach (var show in _restClient.GetShows(latestShowId))
                {
                    _logger.LogInformation($"Adding show: {show.name}");

                    await _storageService.CreateOrUpdateShowAsync(show);
                };

                List<Show> shows;
                while ((shows = await _storageService.GetShowsLimitedAsync(s => s.cast == null)).Count() > 0)
                {
                    foreach (var show in shows)
                    {
                        List<int> castForShow = new();
                        await foreach (Person person in _restClient.GetPersonsForShow(show.id))
                        {
                            _logger.LogInformation($"Adding person: {person.name}");

                            await _storageService.CreateOrUpdatePersonAsync(person);
                            castForShow.Add(person.id);
                        };
                        _logger.LogInformation($"Adding cast for show: {show.name}");

                        show.cast = castForShow;
                        await _storageService.UpdateShowAsync(show.id, show);
                    }
                }

                //run every 10 mins
                await Task.Delay(1000 * 60 * 10, stoppingToken);
            }
        }
    }
}