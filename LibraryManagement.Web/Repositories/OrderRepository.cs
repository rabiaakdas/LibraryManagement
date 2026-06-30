using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Data;
using LibraryManagement.Web.Entity;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly BookContext _context;

        public OrderRepository(BookContext context)
        {
            _context = context;
        }

        public IQueryable<Order> Query()
        {
            return _context.Orders.AsQueryable();
        }

        public IQueryable<Order> QueryWithItems()
        {
            return _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Address)
                .AsQueryable();
        }

        public IQueryable<OrderItem> QueryItems()
        {
            return _context.OrderItems.AsQueryable();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task<Order> GetByIdWithItemsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Address)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public void Add(Order order)
        {
            _context.Orders.Add(order);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
