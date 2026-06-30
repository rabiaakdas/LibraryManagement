using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Repositories
{
    public interface IUserRepository
    {
        IQueryable<User> Query();
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
        Task<int?> GetUserIdByEmailAsync(string email);
        void Add(User user);
        Task SaveChangesAsync();
    }
}
