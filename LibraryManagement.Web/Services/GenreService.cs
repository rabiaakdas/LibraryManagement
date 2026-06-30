using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LibraryManagement.Web.Areas.Admin.Models;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models.Api;
using LibraryManagement.Web.Mappings;
using LibraryManagement.Web.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Provides category and genre business logic for public, admin, and API flows.
    /// </summary>
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genres;
        private readonly IMapper _mapper;

        public GenreService(IGenreRepository genres, IMapper mapper = null)
        {
            _genres = genres;
            _mapper = mapper ?? new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
        }

        public async Task<List<Genre>> GetAllOrderedAsync()
        {
            return await _genres.GetAllOrderedAsync();
        }

        public async Task<AdminGenreListViewModel> GetAdminGenreListAsync()
        {
            return new AdminGenreListViewModel
            {
                Genres = await _genres.QueryWithBooks()
                    .OrderBy(g => g.GenreId)
                    .Select(g => new AdminGenreListItemViewModel
                    {
                        GenreId = g.GenreId,
                        Name = g.Name,
                        BookCount = g.Books.Count
                    })
                    .ToListAsync()
            };
        }

        public async Task<Genre> GetGenreWithBooksAsync(int id)
        {
            return await _genres.GetByIdWithBooksAsync(id);
        }

        public async Task<AdminGenreFormViewModel> GetEditGenreModelAsync(int id)
        {
            var genre = await _genres.GetByIdAsync(id);
            if (genre == null)
            {
                return null;
            }

            return new AdminGenreFormViewModel
            {
                GenreId = genre.GenreId,
                Name = genre.Name
            };
        }

        public async Task<bool> NameExistsAsync(string name, int? excludedGenreId = null)
        {
            return await _genres.NameExistsAsync(name, excludedGenreId);
        }

        public async Task CreateGenreAsync(AdminGenreFormViewModel model)
        {
            _genres.Add(new Genre { Name = model.Name.Trim() });
            await _genres.SaveChangesAsync();
        }

        public async Task<bool> UpdateGenreAsync(int id, AdminGenreFormViewModel model)
        {
            var genre = await _genres.GetByIdAsync(id);
            if (genre == null)
            {
                return false;
            }

            genre.Name = model.Name.Trim();
            await _genres.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Error)> DeleteGenreAsync(int id)
        {
            var genre = await _genres.GetByIdWithBooksAsync(id);
            if (genre == null)
            {
                return (false, "Kategori bulunamadi.");
            }

            if (genre.Books.Any())
            {
                return (false, "Bu kategoriye bağlı kitaplar olduğu için silinemez.");
            }

            _genres.Remove(genre);
            await _genres.SaveChangesAsync();
            return (true, null);
        }

        public async Task<List<GenreDto>> GetApiGenresAsync()
        {
            var genres = await _genres.QueryWithBooks()
                .OrderBy(g => g.Name)
                .ToListAsync();

            return _mapper.Map<List<GenreDto>>(genres);
        }

        public async Task<GenreDto> GetApiGenreAsync(int id)
        {
            var genre = await _genres.QueryWithBooks()
                .FirstOrDefaultAsync(g => g.GenreId == id);

            return genre == null ? null : _mapper.Map<GenreDto>(genre);
        }
    }
}
