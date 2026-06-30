using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;

namespace LibraryManagement.Web.Services
{
    public interface ICouponService
    {
        Task<List<Coupon>> GetAllAsync();
        Task<Coupon> GetByIdAsync(int id);
        Task CreateAsync(Coupon coupon);
        Task<bool> UpdateAsync(int id, Coupon coupon);
        Task<bool> DeactivateAsync(int id);
        Task<CouponResultViewModel> ValidateCouponAsync(string code, decimal subTotal);
        Task MarkAsUsedAsync(string code);
    }
}
