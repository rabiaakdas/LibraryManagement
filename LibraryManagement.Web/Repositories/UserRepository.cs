using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BookContext _context;

        public UserRepository(BookContext context)
        {
            _context = context;
        }

        public IQueryable<User> Query()
        {
            return _context.Users.AsQueryable();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<int?> GetUserIdByEmailAsync(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email)
                .Select(u => (int?)u.UserId)
                .FirstOrDefaultAsync();
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
