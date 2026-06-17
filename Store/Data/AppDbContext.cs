using Microsoft.EntityFrameworkCore;
using Store.Models;

namespace Store.Data
{
	public class AppDbContext : DbContext
	{

		public DbSet<User> Users { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public DbSet<Review> Reviews { get; set; }
		public DbSet<ViewLog> Views { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{

			optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=StoreDb;Trusted_Connection=True;TrustServerCertificate=True;");
		}
	}
}