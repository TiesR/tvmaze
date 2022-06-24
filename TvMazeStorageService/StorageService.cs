using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;
using TvMazeStorageService.Models;

namespace TvMazeStorageService
{
    public class StorageService : IStorageService
    {

        private readonly IMongoCollection<Show> _showsCollection;
        private readonly IMongoCollection<Person> _personsCollection;

        public StorageService(
                IOptions<TvMazeDatabaseSettings> tvMazeDatabaseSettings)

        {
            var mongoClient = new MongoClient(
                tvMazeDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                tvMazeDatabaseSettings.Value.DatabaseName);
            try
            {
                mongoDatabase.CreateCollection(tvMazeDatabaseSettings.Value.ShowsCollectionName);
                mongoDatabase.CreateCollection(tvMazeDatabaseSettings.Value.PersonsCollectionName);
            }
            catch
            {

            }


            _showsCollection = mongoDatabase.GetCollection<Show>(
                tvMazeDatabaseSettings.Value.ShowsCollectionName);
            _personsCollection = mongoDatabase.GetCollection<Person>(
                tvMazeDatabaseSettings.Value.PersonsCollectionName);
        }


        public async Task<List<Show>> GetShowsLimitedAsync(Expression<Func<Show, bool>>? query = null, int maxResults = 1000) =>
            await _showsCollection.Find(query ?? (_ => true)).Limit(maxResults).ToListAsync();

        public async Task<List<Show>> GetShowsPagedAsync(int pageSize, int pageNumber) =>
            await _showsCollection.Find(_ => true).SortBy(s => s.id).Skip(pageSize * (pageNumber -1)).Limit(pageSize).ToListAsync();

        public async Task<List<Person>> GetPersonsSortedByBirthdayAsync(List<int> personIds) =>
            await _personsCollection.Find(p => personIds.Contains(p.id)).SortByDescending(p => p.birthday).ToListAsync();

        public async Task<Show> GetMostRecentShowAsync() =>
            await _showsCollection.Find(_ => true).SortByDescending(s => s.id).FirstOrDefaultAsync();

        //    public async Task<Show?> GetAsync(int id) =>
        //        await _showsCollection.Find(x => x.id == id).FirstOrDefaultAsync();

        public async Task CreateOrUpdateShowAsync(Show newShow)
        {
            var existingShows = _showsCollection.Find(x => x.id == newShow.id);
            if (existingShows.CountDocuments() == 0)
            {
                await _showsCollection.InsertOneAsync(newShow);
            }
            else
            {
                newShow.cast = existingShows.First().cast;
                await _showsCollection.ReplaceOneAsync(x => x.id == newShow.id, newShow);
            }
        }

        public async Task UpdateShowAsync(int id, Show updatedShow) =>
            await _showsCollection.ReplaceOneAsync(x => x.id == id, updatedShow);

        public async Task CreateOrUpdatePersonAsync(Person newPerson)
        {
            var existingPersons = _personsCollection.Find(x => x.id == newPerson.id);
            if (existingPersons.CountDocuments() == 0)
            {
                await _personsCollection.InsertOneAsync(newPerson);
            }
            else
            {
                await _personsCollection.ReplaceOneAsync(x => x.id == newPerson.id, newPerson);
            }
        }
    }
}