using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TvMazeStorageService.Models
{
    public class Person
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? MongoId { get; set; }

        public string? name { get; set; }

        public int id { get; set; }

        public string? birthday { get; set; }

        [BsonExtraElements]
        public BsonDocument Metadata { get; set; } = null!;

    }
}
