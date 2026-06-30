using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Repositories;

namespace LibraryManagement.Web.Services
{
    /// <summary>
    /// Provides address management operations through the address repository.
    /// </summary>
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addresses;

        public AddressService(IAddressRepository addresses)
        {
            _addresses = addresses;
        }

        public async Task<List<AddressViewModel>> GetUserAddressesAsync(string userId)
        {
            var addresses = await _addresses.GetByUserAsync(userId);
            return addresses.Select(a => new AddressViewModel
            {
                Id = a.Id,
                Title = a.Title,
                City = a.City,
                District = a.District,
                ZipCode = a.ZipCode,
                FullAddress = a.FullAddress
            }).ToList();
        }

        public async Task CreateAddressAsync(AddressViewModel model, string userId)
        {
            _addresses.Add(new Address
            {
                Title = model.Title,
                City = model.City,
                District = model.District,
                ZipCode = model.ZipCode,
                FullAddress = model.FullAddress,
                UserId = userId
            });

            await _addresses.SaveChangesAsync();
        }

        public async Task<Address> GetAddressAsync(int id)
        {
            return await _addresses.GetByIdAsync(id);
        }

        public async Task<AddressViewModel> GetAddressModelAsync(int id)
        {
            var address = await _addresses.GetByIdAsync(id);
            if (address == null)
            {
                return null;
            }

            return new AddressViewModel
            {
                Id = address.Id,
                Title = address.Title,
                City = address.City,
                District = address.District,
                ZipCode = address.ZipCode,
                FullAddress = address.FullAddress
            };
        }

        public async Task<bool> UpdateAddressAsync(AddressViewModel model)
        {
            var address = await _addresses.GetByIdAsync(model.Id);
            if (address == null)
            {
                return false;
            }

            address.Title = model.Title;
            address.City = model.City;
            address.District = model.District;
            address.ZipCode = model.ZipCode;
            address.FullAddress = model.FullAddress;

            await _addresses.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAddressAsync(int id)
        {
            var address = await _addresses.GetByIdAsync(id);
            if (address == null)
            {
                return false;
            }

            _addresses.Remove(address);
            await _addresses.SaveChangesAsync();
            return true;
        }
    }
}
