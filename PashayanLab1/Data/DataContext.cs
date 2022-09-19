using Microsoft.EntityFrameworkCore;
using PashayanLab1.Models;

namespace PashayanLab1.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
