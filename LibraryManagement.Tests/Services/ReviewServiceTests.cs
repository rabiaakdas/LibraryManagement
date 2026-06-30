using LibraryManagement.Tests.Helpers;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models.Api;
using LibraryManagement.Web.Repositories;
using LibraryManagement.Web.Services;
using Moq;

namespace LibraryManagement.Tests.Services;

public class ReviewServiceTests
{
    [Fact]
    public async Task AddReviewAsync_WhenUserAlreadyReviewed_ReturnsFalse()
    {
        var reviewRepository = new Mock<IReviewRepository>();
        var bookRepository = new Mock<IBookRepository>();
        bookRepository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(new List<Book>
        {
            new() { BookId = 1, Title = "Dune", Author = "Frank Herbert" }
        }));
        reviewRepository.Setup(r => r.ExistsForBookAndUserAsync(1, 10)).ReturnsAsync(true);

        var service = new ReviewService(reviewRepository.Object, bookRepository.Object);

        var result = await service.AddReviewAsync(new LibraryManagement.Web.Models.BookReviewCreateViewModel
        {
            BookId = 1,
            Rating = 5,
            Comment = "Great"
        }, 10);

        Assert.False(result);
        reviewRepository.Verify(r => r.Add(It.IsAny<BookReview>()), Times.Never);
    }

    [Fact]
    public async Task GetApiBookAsync_CalculatesAverageRatingCorrectly()
    {
        var bookRepository = new Mock<IBookRepository>();
        var genreRepository = new Mock<IGenreRepository>();
        var reviewRepository = new Mock<IReviewRepository>();
        var book = new Book { BookId = 1, Title = "Dune", Author = "Frank Herbert", Price = 100, Stock = 5 };

        bookRepository.Setup(r => r.GetByIdWithGenresAsync(1)).ReturnsAsync(book);
        reviewRepository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(new List<BookReview>
        {
            new() { BookId = 1, Rating = 5, Comment = "A" },
            new() { BookId = 1, Rating = 3, Comment = "B" }
        }));

        var service = new BookService(bookRepository.Object, genreRepository.Object, reviewRepository.Object);

        var result = await service.GetApiBookAsync(1);

        Assert.Equal(4, result!.RatingAverage);
        Assert.Equal(2, result.ReviewCount);
    }
}
