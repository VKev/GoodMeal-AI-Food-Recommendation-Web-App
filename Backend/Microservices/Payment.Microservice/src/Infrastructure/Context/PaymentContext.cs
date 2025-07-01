using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class PaymentContext : DbContext
{
    public PaymentContext()
    {
    }

    public PaymentContext(DbContextOptions<PaymentContext> options)
        : base(options)
    {
    }
    

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("pgcrypto")
            .HasPostgresExtension("uuid-ossp");
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}