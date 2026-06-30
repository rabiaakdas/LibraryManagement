using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Repositories
{
    public interface IFavoriteRepository
    {
        Task<bool> ExistsAsync(int bookId, string userId);
        Task<List<Favorite>> GetByUserWithBookAsync(string userId);
        void Add(Favorite favorite);
        Task SaveChangesAsync();
    }
}
