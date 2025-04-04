namespace MoviesAPI.Dtos
{
    public class MovieDto
    {
        [MaxLength(250)]
        public string Title { get; set; }
        public int Year { get; set; }
        public double Rate { get; set; }
        
        public string Storeline { get; set; }
        public IFormFile? poster { get; set; }
        public byte? GenreId { get; set; }
    }
}
