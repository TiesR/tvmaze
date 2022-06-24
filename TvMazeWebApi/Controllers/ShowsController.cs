using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TvMazeStorageService;
using TvMazeWebApi.Models;

namespace TvMazeWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShowsController : ControllerBase
    {
        private readonly ILogger<ShowsController> _logger;
        private readonly IStorageService _storageService;

        public ShowsController(ILogger<ShowsController> logger, IStorageService storageService)
        {
            _logger = logger;
            _storageService = storageService;
        }

        [HttpGet(Name = "GetShows")]
        public async Task<IEnumerable<ShowModel>> Get([FromQuery] ShowsParameters parameters)
        {
            var shows = await _storageService.GetShowsPagedAsync(parameters.PageSize, parameters.PageNumber);

            List<int> personList = shows.SelectMany(s => s.cast ?? new List<int>()).ToList();

            var persons = await _storageService.GetPersonsSortedByBirthdayAsync(personList);

            return shows.Select(s => new ShowModel()
            {
                id = s.id,
                name = s.name,
                cast = persons.Where(p => (s.cast ?? new List<int>()).Any(c => p.id == c))
                        .Select(m => new PersonModel()
                        {
                            id = m.id,
                            name = m.name,
                            birthday = m.birthday
                        })
                        .ToList()
            });
        }
    }
}