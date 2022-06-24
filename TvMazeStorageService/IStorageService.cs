using System.Linq.Expressions;
using TvMazeStorageService.Models;

namespace TvMazeStorageService
{
    public interface IStorageService
    {
        Task CreateOrUpdatePersonAsync(Person newPerson);
        Task CreateOrUpdateShowAsync(Show newShow);
        Task<Show> GetMostRecentShowAsync();
        Task<List<Show>> GetShowsLimitedAsync(Expression<Func<Show, bool>>? query = null, int maxResults = 1000);
        Task UpdateShowAsync(int id, Show updatedShow);
        Task<List<Show>> GetShowsPagedAsync(int pageSize, int pageNumber);
        Task<List<Person>> GetPersonsSortedByBirthdayAsync(List<int> personIds);
    }
}