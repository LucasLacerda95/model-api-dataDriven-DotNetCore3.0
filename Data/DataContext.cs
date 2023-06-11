using Microsoft.EntityFrameworkCore;
using Shop.Models;

namespace Shop.Data
{

    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options){}//Representação do banco

        public DbSet<Product> Products { get; set; }//Representação das tabelas

        public DbSet<Category> Categories { get; set; }
        
        public DbSet<User> Users { get; set; }
        
    }
}