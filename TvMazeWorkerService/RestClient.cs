using MongoDB.Bson;
using System.Net.Http.Headers;
using System.Text.Json;
using TvMazeStorageService.Models;

namespace TvMazeWorkerService
{
    internal class RestClient : IRestClient
    {
        private readonly HttpClient _client = new HttpClient();
        private DateTime _lastCall = DateTime.UtcNow;

        public async IAsyncEnumerable<Show> GetShows(int lastId = 0)
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            int currentPage = Convert.ToInt32(Math.Floor(lastId / 250.0));
            HttpResponseMessage httpResponse;
            
            while ((httpResponse = await GetAsyncWithRateLimit($"https://api.tvmaze.com/shows?page={currentPage}")).IsSuccessStatusCode)
            {
                string jsonString = await httpResponse.Content.ReadAsStringAsync() ?? "";

                using (JsonDocument document = JsonDocument.Parse(jsonString))
                {
                    JsonElement showsElement = document.RootElement;
                    foreach (JsonElement showElement in showsElement.EnumerateArray())
                    {
                        Show? show = showElement.Deserialize<Show>();
                        if (show != null && show.id > 0 && !string.IsNullOrEmpty(show.name))
                        {
                            show.Metadata = BsonDocument.Parse(showElement.ToString());
                            yield return show;
                        }
                        else
                        {
                            throw new JsonException("Parsing error: could not deserialize Show JSON.");
                        }
                    }
                }
                currentPage++;
            }
        }

        public async IAsyncEnumerable<Person> GetPersonsForShow(int showId)
        {
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage httpResponse = await GetAsyncWithRateLimit($"https://api.tvmaze.com/shows/{showId}/cast");

            if (!httpResponse.IsSuccessStatusCode) yield break;

            string jsonString = await httpResponse.Content.ReadAsStringAsync() ?? "";

            using (JsonDocument document = JsonDocument.Parse(jsonString))
            {
                JsonElement castElement = document.RootElement;
                IEnumerable<JsonElement> personsElement = castElement.EnumerateArray()
                    .Where(castMember => castMember.TryGetProperty("person", out JsonElement _))
                    .Select(castMember => castMember.GetProperty("person"));

                foreach (JsonElement personElement in personsElement)
                {
                    Person? person = personElement.Deserialize<Person>();
                    if (person != null && person.id > 0 && !string.IsNullOrEmpty(person.name))
                    {
                        person.Metadata = BsonDocument.Parse(personElement.ToString());
                        yield return person;
                    }
                    else
                    {
                        throw new JsonException("Parsing error: could not deserialize Person JSON.");
                    }
                }
            }
        }
        private async Task<HttpResponseMessage> GetAsyncWithRateLimit(string requestUri)
        {
            TimeSpan timeSpanSinceLastCall = DateTime.UtcNow - _lastCall;
            await Task.Delay(Math.Max(500 - timeSpanSinceLastCall.Milliseconds, 0));
            _lastCall = DateTime.UtcNow;
            return await _client.GetAsync(requestUri);
        }
    }
}
