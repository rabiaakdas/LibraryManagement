using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly BookContext _context;

        public AddressRepository(BookContext context)
        {
            _context = context;
        }

        public async Task<List<Address>> GetByUserAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<Address> GetByIdAsync(int id)
        {
            return await _context.Addresses.FindAsync(id);
        }

        public void Add(Address address)
        {
            _context.Addresses.Add(address);
        }

        public void Remove(Address address)
        {
            _context.Addresses.Remove(address);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
