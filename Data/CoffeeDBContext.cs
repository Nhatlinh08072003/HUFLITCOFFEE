using Microsoft.EntityFrameworkCore;
namespace HUFLITCOFFEE.web.Data
{
    public class CoffeeDBContext : DbContext
    {
        public CoffeeDBContext(DbContextOptions options) : base(options)
        {
        }
    }
}