namespace LibraryManagement.Web.Models.Api
{
    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int BookCount { get; set; }
    }
}
