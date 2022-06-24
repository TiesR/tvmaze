namespace TvMazeWebApi.Models
{
    public class ShowModel
    {
        public int id { get; set; }
        public string? name { get; set; }
        public List<PersonModel>? cast { get; set; }

   }
}
