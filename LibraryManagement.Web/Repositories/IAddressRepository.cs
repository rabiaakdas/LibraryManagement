using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Repositories
{
    public interface IAddressRepository
    {
        Task<List<Address>> GetByUserAsync(string userId);
        Task<Address> GetByIdAsync(int id);
        void Add(Address address);
        void Remove(Address address);
        Task SaveChangesAsync();
    }
}
