using LibraryManagement.Tests.Helpers;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Repositories;
using LibraryManagement.Web.Services;
using Moq;

namespace LibraryManagement.Tests.Services;

public class GenreServiceTests
{
    [Fact]
    public async Task GetApiGenresAsync_ReturnsGenreList()
    {
        var genres = new List<Genre>
        {
            new() { GenreId = 1, Name = "Roman", Books = new HashSet<Book> { new() { BookId = 1, Title = "Book", Author = "Author" } } },
            new() { GenreId = 2, Name = "Bilim", Books = new HashSet<Book>() }
        };

        var repository = new Mock<IGenreRepository>();
        repository.Setup(r => r.QueryWithBooks()).Returns(AsyncQueryable.Create(genres));
        var service = new GenreService(repository.Object);

        var result = await service.GetApiGenresAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result.First(g => g.Name == "Roman").BookCount);
    }

    [Fact]
    public async Task NameExistsAsync_WhenGenreExists_ReturnsTrue()
    {
        var repository = new Mock<IGenreRepository>();
        repository.Setup(r => r.NameExistsAsync("Roman", null)).ReturnsAsync(true);
        var service = new GenreService(repository.Object);

        var result = await service.NameExistsAsync("Roman");

        Assert.True(result);
    }
}
