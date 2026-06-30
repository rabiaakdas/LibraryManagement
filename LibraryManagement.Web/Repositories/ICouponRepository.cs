using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Repositories
{
    public interface ICouponRepository
    {
        IQueryable<Coupon> Query();
        Task<Coupon> GetByIdAsync(int id);
        Task<Coupon> GetByCodeAsync(string code);
        void Add(Coupon coupon);
        void Remove(Coupon coupon);
        Task SaveChangesAsync();
    }
}
