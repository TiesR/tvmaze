using TvMazeStorageService.Models;

namespace TvMazeWorkerService
{
    public interface IRestClient
    {
        IAsyncEnumerable<Person> GetPersonsForShow(int showId);
        IAsyncEnumerable<Show> GetShows(int lastId = 0);
    }
}