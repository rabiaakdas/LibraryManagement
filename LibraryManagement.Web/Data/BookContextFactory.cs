using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LibraryManagement.Web.Data
{
    public class BookContextFactory : IDesignTimeDbContextFactory<BookContext>
    {
        public BookContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BookContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=LibraryManagementDb;Trusted_Connection=True;TrustServerCertificate=True;");
            return new BookContext(optionsBuilder.Options);
        }
    }
}
