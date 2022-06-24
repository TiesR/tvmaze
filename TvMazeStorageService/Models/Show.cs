using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvMazeStorageService.Models
{
    public class Show
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? MongoId { get; set; }

        public string? name { get; set; }

        public int id { get; set; }

        [BsonIgnoreIfDefault]
        public List<int>? cast { get; set; }

        [BsonExtraElements]
        public BsonDocument Metadata { get; set; } = null!;

    }
}
