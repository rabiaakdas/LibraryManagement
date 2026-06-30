using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories
{
    public class CouponRepository : ICouponRepository
    {
        private readonly BookContext _context;

        public CouponRepository(BookContext context)
        {
            _context = context;
        }

        public IQueryable<Coupon> Query()
        {
            return _context.Coupons.AsQueryable();
        }

        public async Task<Coupon> GetByIdAsync(int id)
        {
            return await _context.Coupons.FindAsync(id);
        }

        public async Task<Coupon> GetByCodeAsync(string code)
        {
            var normalizedCode = code.Trim().ToUpper();
            return await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToUpper() == normalizedCode);
        }

        public void Add(Coupon coupon)
        {
            _context.Coupons.Add(coupon);
        }

        public void Remove(Coupon coupon)
        {
            _context.Coupons.Remove(coupon);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
