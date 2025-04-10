using Assignment1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Assignment1.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    
    public DbSet<Category> Categories{ get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Must ensure Identity's configuration are applied first
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("Identity");
    
         
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("User");
        });
        modelBuilder.Entity<IdentityRole>(entity =>
        {
            entity.ToTable("Role");
        });
        modelBuilder.Entity<IdentityUserRole<string>>(entity =>
        {
            entity.ToTable("UserRoles");
        });
        modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
        {
            entity.ToTable("UserClaims");
        });
        modelBuilder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        });
        modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        {
            entity.ToTable("UserLogins");
        });
        modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("UserRoleClaims");
        });

    }

    /*public DbSet<User> Users { get; set; }  UNDER NEW MANAGEMENT*/
}