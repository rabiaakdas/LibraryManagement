using LibraryManagement.Tests.Helpers;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Models.Api;
using LibraryManagement.Web.Repositories;
using LibraryManagement.Web.Services;
using Moq;

namespace LibraryManagement.Tests.Services;

public class BookServiceTests
{
    private static BookService CreateService(
        List<Book> books,
        List<Genre>? genres = null,
        List<BookReview>? reviews = null)
    {
        var bookRepository = new Mock<IBookRepository>();
        var genreRepository = new Mock<IGenreRepository>();
        var reviewRepository = new Mock<IReviewRepository>();

        bookRepository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(books));
        bookRepository.Setup(r => r.QueryWithGenres()).Returns(AsyncQueryable.Create(books));
        bookRepository.Setup(r => r.GetByIdWithGenresAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) => books.FirstOrDefault(b => b.BookId == id));

        genreRepository.Setup(r => r.GetAllOrderedAsync()).ReturnsAsync(genres ?? new List<Genre>());
        genreRepository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(genres ?? new List<Genre>()));
        genreRepository.Setup(r => r.GetByIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync((List<int> ids) => (genres ?? new List<Genre>()).Where(g => ids.Contains(g.GenreId)).ToList());

        reviewRepository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(reviews ?? new List<BookReview>()));
        reviewRepository.Setup(r => r.QueryWithUser()).Returns(AsyncQueryable.Create(reviews ?? new List<BookReview>()));

        return new BookService(bookRepository.Object, genreRepository.Object, reviewRepository.Object);
    }

    [Fact]
    public async Task GetFilteredBooksAsync_ReturnsBooks()
    {
        var service = CreateService(new List<Book>
        {
            new() { BookId = 1, Title = "Dune", Author = "Frank Herbert", Price = 100, Stock = 3 },
            new() { BookId = 2, Title = "1984", Author = "George Orwell", Price = 80, Stock = 5 }
        });

        var result = await service.GetFilteredBooksAsync(new BookFilterViewModel());

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Books.Count);
    }

    [Fact]
    public async Task GetApiBookAsync_WhenBookExists_ReturnsBook()
    {
        var service = CreateService(new List<Book>
        {
            new() { BookId = 1, Title = "Dune", Author = "Frank Herbert", Price = 100, Stock = 3 }
        });

        var result = await service.GetApiBookAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Dune", result!.Title);
    }

    [Fact]
    public async Task GetApiBookAsync_WhenBookDoesNotExist_ReturnsNull()
    {
        var service = CreateService(new List<Book>());

        var result = await service.GetApiBookAsync(99);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetFilteredBooksAsync_FiltersByAuthorAndSortsByPriceDescending()
    {
        var service = CreateService(new List<Book>
        {
            new() { BookId = 1, Title = "Animal Farm", Author = "George Orwell", Price = 50, Stock = 2 },
            new() { BookId = 2, Title = "1984", Author = "George Orwell", Price = 80, Stock = 4 },
            new() { BookId = 3, Title = "Dune", Author = "Frank Herbert", Price = 100, Stock = 1 }
        });

        var result = await service.GetFilteredBooksAsync(new BookFilterViewModel
        {
            Search = "orwell",
            Sort = "price_desc"
        });

        Assert.Equal(2, result.Books.Count);
        Assert.Equal("1984", result.Books.First().Title);
    }
    [Theory]
    [InlineData(10, "InStock")]
    [InlineData(5, "LowStock")]
    [InlineData(1, "LowStock")]
    [InlineData(0, "OutOfStock")]
    public void GetStockStatus_ReturnsExpectedStatus(int stock, string expectedStatus)
    {
        var service = CreateService(new List<Book>());

        var result = service.GetStockStatus(stock);

        Assert.Equal(expectedStatus, result);
    }

    [Fact]
    public async Task GetAdminBookListAsync_WhenLowStockFilterSelected_ReturnsOnlyLowStockBooks()
    {
        var service = CreateService(new List<Book>
        {
            new() { BookId = 1, Title = "In Stock", Author = "Author", Stock = 10 },
            new() { BookId = 2, Title = "Low Stock", Author = "Author", Stock = 5 },
            new() { BookId = 3, Title = "Out Of Stock", Author = "Author", Stock = 0 }
        });

        var result = await service.GetAdminBookListAsync("low_stock");

        Assert.Single(result.Books);
        Assert.Equal("Low Stock", result.Books.First().Title);
        Assert.Equal("low_stock", result.StockFilter);
    }
}
