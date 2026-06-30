using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Models.Api;
using LibraryManagement.Web.Mappings;
using LibraryManagement.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Provides book-related business logic for catalog, admin, dashboard, and API features.
    /// </summary>
    public class BookService : IBookService
    {
        private readonly IBookRepository _books;
        private readonly IGenreRepository _genres;
        private readonly IReviewRepository _reviews;
        private readonly IMapper _mapper;

        public BookService(IBookRepository books, IGenreRepository genres, IReviewRepository reviews, IMapper mapper = null)
        {
            _books = books;
            _genres = genres;
            _reviews = reviews;
            _mapper = mapper ?? new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        }

        public async Task<HomePageViewModel> GetHomePageAsync()
        {
            return new HomePageViewModel { PopularBooks = await _books.GetAllAsync() };
        }

        public async Task<BookFilterViewModel> GetFilteredBooksAsync(BookFilterViewModel filter)
        {
            if (filter.Page < 1)
            {
                filter.Page = 1;
            }

            filter.PageSize = 9;
            var booksQuery = _books.QueryWithGenres();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchTerm = filter.Search.ToLower();
                booksQuery = booksQuery.Where(b =>
                    b.Title.ToLower().Contains(searchTerm) ||
                    b.Author.ToLower().Contains(searchTerm));
            }

            if (filter.CategoryId.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.Genres.Any(g => g.GenreId == filter.CategoryId.Value));
            }

            if (filter.MinPrice.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.Price <= filter.MaxPrice.Value);
            }

            if (filter.InStockOnly)
            {
                booksQuery = booksQuery.Where(b => b.Stock > 0);
            }

            filter.TotalItems = await booksQuery.CountAsync();
            filter.TotalPages = (int)Math.Ceiling(filter.TotalItems / (double)filter.PageSize);

            if (filter.TotalPages > 0 && filter.Page > filter.TotalPages)
            {
                filter.Page = filter.TotalPages;
            }

            var books = await booksQuery.ToListAsync();
            var turkishComparer = StringComparer.Create(new CultureInfo("tr-TR"), ignoreCase: true);

            books = filter.Sort switch
            {
                "name_desc" => books.OrderByDescending(b => b.Title, turkishComparer).ToList(),
                "author_asc" => books.OrderBy(b => b.Author, turkishComparer).ToList(),
                "price_asc" => books.OrderBy(b => b.Price).ToList(),
                "price_desc" => books.OrderByDescending(b => b.Price).ToList(),
                "newest" => books.OrderByDescending(b => b.BookId).ToList(),
                "stock_desc" => books.OrderByDescending(b => b.Stock).ToList(),
                _ => books.OrderBy(b => b.Title, turkishComparer).ToList()
            };

            filter.Books = books
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var bookIds = filter.Books.Select(b => b.BookId).ToList();
            filter.ReviewSummaries = await _reviews.Query()
                .Where(r => bookIds.Contains(r.BookId))
                .GroupBy(r => r.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    AverageRating = g.Average(r => r.Rating),
                    ReviewCount = g.Count()
                })
                .ToDictionaryAsync(
                    x => x.BookId,
                    x => new BookReviewSummaryViewModel
                    {
                        AverageRating = x.AverageRating,
                        ReviewCount = x.ReviewCount
                    });

            filter.Categories = await _genres.GetAllOrderedAsync();
            return filter;
        }

        public async Task<BookDetailsViewModel> GetBookDetailsAsync(int id, int? currentUserId, bool isAuthenticated)
        {
            var book = await _books.GetByIdWithGenresAsync(id);
            if (book == null)
            {
                return null;
            }

            var reviews = await _reviews.QueryWithUser()
                .Where(r => r.BookId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var hasReviewed = currentUserId.HasValue && reviews.Any(r => r.UserId == currentUserId.Value);

            return new BookDetailsViewModel
            {
                Book = book,
                Reviews = reviews,
                AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
                ReviewCount = reviews.Count,
                CanReview = isAuthenticated && !hasReviewed,
                HasReviewed = hasReviewed,
                NewReview = new BookReviewCreateViewModel { BookId = id }
            };
        }

        public async Task<AdminBookListViewModel> GetAdminBookListAsync(string stockFilter = null)
        {
            var query = _books.QueryWithGenres();

            query = stockFilter switch
            {
                "in_stock" => query.Where(b => b.Stock > 5),
                "low_stock" => query.Where(b => b.Stock > 0 && b.Stock <= 5),
                "out_of_stock" => query.Where(b => b.Stock <= 0),
                _ => query
            };

            return new AdminBookListViewModel
            {
                StockFilter = stockFilter,
                Books = await query.OrderBy(b => b.BookId).ToListAsync()
            };
        }

        public string GetStockStatus(int stock)
        {
            if (stock <= 0)
            {
                return "OutOfStock";
            }

            return stock <= 5 ? "LowStock" : "InStock";
        }

        public async Task<Book> GetBookWithGenresAsync(int id)
        {
            return await _books.GetByIdWithGenresAsync(id);
        }

        public async Task<AdminBookFormViewModel> GetCreateBookModelAsync()
        {
            return new AdminBookFormViewModel
            {
                Genres = await GetGenreCheckboxesAsync(new List<int>())
            };
        }

        public async Task<AdminBookFormViewModel> GetEditBookModelAsync(int id)
        {
            var book = await _books.GetByIdWithGenresAsync(id);
            if (book == null)
            {
                return null;
            }

            var selectedGenreIds = book.Genres.Select(g => g.GenreId).ToList();
            return new AdminBookFormViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                ImageUrl = book.ImageUrl,
                PageCount = book.PageCount,
                Price = book.Price,
                Stock = book.Stock,
                SelectedGenreIds = selectedGenreIds,
                Genres = await GetGenreCheckboxesAsync(selectedGenreIds)
            };
        }

        public async Task CreateBookAsync(AdminBookFormViewModel model)
        {
            var selectedGenres = await _genres.GetByIdsAsync(model.SelectedGenreIds);
            var book = new Book
            {
                Title = model.Title,
                Author = model.Author,
                ImageUrl = model.ImageUrl,
                PageCount = model.PageCount,
                Price = model.Price,
                Stock = model.Stock,
                Genres = selectedGenres.ToHashSet()
            };

            _books.Add(book);
            await _books.SaveChangesAsync();
        }

        public async Task<bool> UpdateBookAsync(int id, AdminBookFormViewModel model)
        {
            var book = await _books.GetByIdWithGenresAsync(id);
            if (book == null)
            {
                return false;
            }

            book.Title = model.Title;
            book.Author = model.Author;
            book.ImageUrl = model.ImageUrl;
            book.PageCount = model.PageCount;
            book.Price = model.Price;
            book.Stock = model.Stock;

            var selectedGenres = await _genres.GetByIdsAsync(model.SelectedGenreIds);
            book.Genres.Clear();
            foreach (var genre in selectedGenres)
            {
                book.Genres.Add(genre);
            }

            await _books.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Error)> DeleteBookAsync(int id)
        {
            var book = await _books.GetByIdWithGenresAsync(id);
            if (book == null)
            {
                return (false, "Kitap bulunamadi.");
            }

            try
            {
                _books.Remove(book);
                await _books.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateException)
            {
                return (false, "Kitap silinemedi. Bu kitap baska kayitlarla iliskili olabilir.");
            }
        }

        public async Task<List<DashboardLowStockBookViewModel>> GetLowStockDashboardAsync()
        {
            var lowStockBooks = await _books.Query()
                .Where(b => b.Stock <= 5)
                .OrderBy(b => b.Stock)
                .ThenBy(b => b.Title)
                .Select(b => new
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    Stock = b.Stock,
                    Price = b.Price
                })
                .ToListAsync();

            return lowStockBooks
                .Select(b => new DashboardLowStockBookViewModel
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    Stock = b.Stock,
                    Price = b.Price,
                    StockStatus = GetStockStatus(b.Stock)
                })
                .ToList();
        }

        public async Task<List<BookDto>> GetApiBooksAsync()
        {
            var books = await _books.QueryWithGenres()
                .OrderBy(b => b.Title)
                .ToListAsync();

            var summaries = await GetReviewSummariesAsync(books.Select(b => b.BookId).ToList());
            return books.Select(b => MapBookDto(b, summaries)).ToList();
        }

        public async Task<BookDto> GetApiBookAsync(int id)
        {
            var book = await _books.GetByIdWithGenresAsync(id);
            if (book == null)
            {
                return null;
            }

            var summaries = await GetReviewSummariesAsync(new List<int> { id });
            return MapBookDto(book, summaries);
        }

        public async Task<BookDto> CreateApiBookAsync(BookUpsertDto model)
        {
            var selectedGenres = await _genres.GetByIdsAsync(model.GenreIds);
            var book = _mapper.Map<Book>(model);
            book.Genres = selectedGenres.ToHashSet();

            _books.Add(book);
            await _books.SaveChangesAsync();
            return await GetApiBookAsync(book.BookId);
        }

        public async Task<(bool Success, BookDto Book)> UpdateApiBookAsync(int id, BookUpsertDto model)
        {
            var book = await _books.GetByIdWithGenresAsync(id);
            if (book == null)
            {
                return (false, null);
            }

            _mapper.Map(model, book);


            var selectedGenres = await _genres.GetByIdsAsync(model.GenreIds);
            book.Genres.Clear();
            foreach (var genre in selectedGenres)
            {
                book.Genres.Add(genre);
            }

            await _books.SaveChangesAsync();
            return (true, await GetApiBookAsync(id));
        }

        private async Task<List<AdminGenreCheckboxViewModel>> GetGenreCheckboxesAsync(List<int> selectedGenreIds)
        {
            return await _genres.Query()
                .OrderBy(g => g.Name)
                .Select(g => new AdminGenreCheckboxViewModel
                {
                    GenreId = g.GenreId,
                    Name = g.Name,
                    IsSelected = selectedGenreIds.Contains(g.GenreId)
                })
                .ToListAsync();
        }

        private async Task<Dictionary<int, BookReviewSummaryViewModel>> GetReviewSummariesAsync(List<int> bookIds)
        {
            return await _reviews.Query()
                .Where(r => bookIds.Contains(r.BookId))
                .GroupBy(r => r.BookId)
                .Select(g => new
                {
                    BookId = g.Key,
                    AverageRating = g.Average(r => r.Rating),
                    ReviewCount = g.Count()
                })
                .ToDictionaryAsync(
                    x => x.BookId,
                    x => new BookReviewSummaryViewModel
                    {
                        AverageRating = x.AverageRating,
                        ReviewCount = x.ReviewCount
                    });
        }

        private BookDto MapBookDto(Book book, Dictionary<int, BookReviewSummaryViewModel> summaries)
        {
            var dto = _mapper.Map<BookDto>(book);
            if (summaries.TryGetValue(book.BookId, out var summary))
            {
                dto.RatingAverage = summary.AverageRating;
                dto.ReviewCount = summary.ReviewCount;
            }

            return dto;
        }
    }
}
