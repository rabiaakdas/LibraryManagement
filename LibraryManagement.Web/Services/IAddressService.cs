using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Defines address operations used by checkout and user address pages.
    /// </summary>
    public interface IAddressService
    {
        Task<List<AddressViewModel>> GetUserAddressesAsync(string userId);
        Task CreateAddressAsync(AddressViewModel model, string userId);
        Task<Address> GetAddressAsync(int id);
        Task<AddressViewModel> GetAddressModelAsync(int id);
        Task<bool> UpdateAddressAsync(AddressViewModel model);
        Task<bool> DeleteAddressAsync(int id);
    }
}
