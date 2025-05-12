using Microsoft.EntityFrameworkCore;

namespace Order_Code.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Agent)
                .WithMany(a => a.Orders) // Relates to Agent.Orders
                .HasForeignKey(o => o.AgentID);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderID);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Item)
                .WithMany() // Item doesn't track OrderDetails (optional)
                .HasForeignKey(od => od.ItemID);

            // Configure primary key for OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => od.OrderDetailID);

            // Ensure required fields (optional, adjust based on your needs)
            modelBuilder.Entity<Order>()
                .Property(o => o.OrderDate)
                .IsRequired(false); // Allows null OrderDate

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Quantity)
                .IsRequired(false); // Allows null Quantity
        }
    }
}