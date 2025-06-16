using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class BusinessRestaurantContext : DbContext
{
    public BusinessRestaurantContext()
    {
    }

    public BusinessRestaurantContext(DbContextOptions<BusinessRestaurantContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Business> Businesses { get; set; }
    public virtual DbSet<BusinessRestaurant> BusinessRestaurants { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("pgcrypto")
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Business>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("businesses_pkey");

            entity.ToTable("businesses");

            entity.HasIndex(e => e.OwnerId, "ix_businesses_owner_id").UseCollation(new[] { "C.utf8" });

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .UseCollation("C.utf8")
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .UseCollation("C.utf8")
                .HasColumnName("created_by");
            entity.Property(e => e.Description)
                .UseCollation("C.utf8")
                .HasColumnName("description");
            entity.Property(e => e.DisableAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("disable_at");
            entity.Property(e => e.DisableBy)
                .UseCollation("C.utf8")
                .HasColumnName("disable_by");
            entity.Property(e => e.Email)
                .UseCollation("C.utf8")
                .HasColumnName("email");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.IsDisable).HasColumnName("is_disable");
            entity.Property(e => e.Name)
                .UseCollation("C.utf8")
                .HasColumnName("name");
            entity.Property(e => e.OwnerId)
                .UseCollation("C.utf8")
                .HasColumnName("owner_id");
            entity.Property(e => e.Phone)
                .UseCollation("C.utf8")
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.Website)
                .UseCollation("C.utf8")
                .HasColumnName("website");
        });

        modelBuilder.Entity<BusinessRestaurant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("business_restaurants_pkey");

            entity.ToTable("business_restaurants");

            entity.HasIndex(e => new { e.BusinessId, e.RestaurantId }, "ix_business_restaurants_business_restaurant_unique").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.BusinessId).HasColumnName("business_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .UseCollation("C.utf8")
                .HasColumnName("created_by");
            entity.Property(e => e.DisableAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("disable_at");
            entity.Property(e => e.DisableBy)
                .UseCollation("C.utf8")
                .HasColumnName("disable_by");
            entity.Property(e => e.IsDisable).HasColumnName("is_disable");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Business).WithMany(p => p.BusinessRestaurants)
                .HasForeignKey(d => d.BusinessId)
                .HasConstraintName("fk_business_restaurants_business_id");
        });
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}