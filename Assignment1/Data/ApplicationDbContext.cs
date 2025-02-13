using Assignment1.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment1.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories{ get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems{ get; set; }
    public DbSet<User> Users { get; set; }
}