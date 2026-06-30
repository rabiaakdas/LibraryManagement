using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;

namespace LibraryManagement.Web.Repositories
{
    public interface IOrderRepository
    {
        IQueryable<Order> Query();
        IQueryable<Order> QueryWithItems();
        IQueryable<OrderItem> QueryItems();
        Task<Order> GetByIdAsync(int id);
        Task<Order> GetByIdWithItemsAsync(int id);
        void Add(Order order);
        Task SaveChangesAsync();
    }
}
