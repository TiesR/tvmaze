using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TvMazeStorageService.Models
{
    public class TvMazeDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string ShowsCollectionName { get; set; } = null!;
        public string PersonsCollectionName { get; set; } = null!;
    }
}
