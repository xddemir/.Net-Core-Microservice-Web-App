using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FreeCourse.Services.Order.Infrastructure;

public class OrderDbContext : DbContext
{
    public const string DEFAULT_SCHEMA = "ordering";

    private readonly IConfiguration _configuration;

    public OrderDbContext()
    {
    }

    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
        
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder 
        optionsBuilder)
    {
        // _configuration.GetSection("ConnectionStrings")["DefaultConnection"]
        optionsBuilder.UseSqlServer("Server=localhost,1444;Database=OrderDb; User=sa; Password=Password12!;TrustServerCertificate=true");
    }
    
    public DbSet<Domain.OrderAggregate.Order> Orders { get; set; }
    public DbSet<Domain.OrderAggregate.OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)  
    {
        modelBuilder.Entity<Domain.OrderAggregate.Order>().ToTable("Orders", DEFAULT_SCHEMA);
        modelBuilder.Entity<Domain.OrderAggregate.OrderItem>().ToTable("OrderItems", DEFAULT_SCHEMA);

        modelBuilder.Entity<Domain.OrderAggregate.OrderItem>().Property(x => x.Price).HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Domain.OrderAggregate.Order>().OwnsOne(o => o.Address).WithOwner();

        base.OnModelCreating(modelBuilder);
    }
}