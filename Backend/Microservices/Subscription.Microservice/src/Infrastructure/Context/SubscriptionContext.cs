using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class SubscriptionContext : DbContext
{
    public SubscriptionContext()
    {
    }

    public SubscriptionContext(DbContextOptions<SubscriptionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Subscription> Subscriptions { get; set; }
    public virtual DbSet<UserSubscription> UserSubscriptions { get; set; }
    public virtual DbSet<SubscriptionPaymentStatus> SubscriptionPaymentStatuses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("pgcrypto")
            .HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("subscriptions_pkey");

            entity.ToTable("subscriptions");

            entity.HasIndex(e => e.Name, "ix_subscriptions_name").IsUnique().UseCollation(new[] { "C.utf8" });

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .UseCollation("C.utf8")
                .HasColumnName("name");
            entity.Property(e => e.Description)
                .UseCollation("C.utf8")
                .HasColumnName("description");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.DurationInMonths)
                .HasColumnName("duration_in_months");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .UseCollation("C.utf8")
                .HasDefaultValue("USD")
                .HasColumnName("currency");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.CreatedBy)
                .UseCollation("C.utf8")
                .HasColumnName("created_by");
            entity.Property(e => e.UpdatedBy)
                .UseCollation("C.utf8")
                .HasColumnName("updated_by");
            entity.Property(e => e.IsDisable)
                .HasDefaultValue(false)
                .HasColumnName("is_disable");
            entity.Property(e => e.DisableAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("disable_at");
            entity.Property(e => e.DisableBy)
                .UseCollation("C.utf8")
                .HasColumnName("disable_by");
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_subscriptions_pkey");

            entity.ToTable("user_subscriptions");

            entity.HasIndex(e => e.UserId, "ix_user_subscriptions_user_id").UseCollation(new[] { "C.utf8" });
            entity.HasIndex(e => new { e.UserId, e.SubscriptionId }, "ix_user_subscriptions_user_subscription");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.UserId)
                .UseCollation("C.utf8")
                .HasColumnName("user_id");
            entity.Property(e => e.SubscriptionId)
                .HasColumnName("subscription_id");
            entity.Property(e => e.StartDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("start_date");
            entity.Property(e => e.EndDate)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("end_date");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
            entity.Property(e => e.CreatedBy)
                .UseCollation("C.utf8")
                .HasColumnName("created_by");
            entity.Property(e => e.UpdatedBy)
                .UseCollation("C.utf8")
                .HasColumnName("updated_by");
            entity.Property(e => e.IsDisable)
                .HasDefaultValue(false)
                .HasColumnName("is_disable");
            entity.Property(e => e.DisableAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("disable_at");
            entity.Property(e => e.DisableBy)
                .UseCollation("C.utf8")
                .HasColumnName("disable_by");

            entity.HasOne(d => d.Subscription).WithMany(p => p.UserSubscriptions)
                .HasForeignKey(d => d.SubscriptionId)
                .HasConstraintName("fk_user_subscriptions_subscription_id");
        });

        modelBuilder.Entity<SubscriptionPaymentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("subscription_payment_statuses_pkey");

            entity.ToTable("subscription_payment_statuses");

            entity.HasIndex(e => e.CorrelationId, "ix_subscription_payment_statuses_correlation_id").IsUnique();
            entity.HasIndex(e => e.OrderId, "ix_subscription_payment_statuses_order_id").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.CorrelationId)
                .HasColumnName("correlation_id");
            entity.Property(e => e.UserId)
                .UseCollation("C.utf8")
                .HasColumnName("user_id");
            entity.Property(e => e.SubscriptionId)
                .HasColumnName("subscription_id");
            entity.Property(e => e.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("amount");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .UseCollation("C.utf8")
                .HasDefaultValue("VND")
                .HasColumnName("currency");
            entity.Property(e => e.OrderId)
                .UseCollation("C.utf8")
                .HasColumnName("order_id");
            entity.Property(e => e.CurrentState)
                .UseCollation("C.utf8")
                .HasColumnName("current_state");
            entity.Property(e => e.PaymentUrl)
                .UseCollation("C.utf8")
                .HasColumnName("payment_url");
            entity.Property(e => e.PaymentUrlCreated)
                .HasDefaultValue(false)
                .HasColumnName("payment_url_created");
            entity.Property(e => e.PaymentCompleted)
                .HasDefaultValue(false)
                .HasColumnName("payment_completed");
            entity.Property(e => e.SubscriptionActivated)
                .HasDefaultValue(false)
                .HasColumnName("subscription_activated");
            entity.Property(e => e.TransactionId)
                .UseCollation("C.utf8")
                .HasColumnName("transaction_id");
            entity.Property(e => e.FailureReason)
                .UseCollation("C.utf8")
                .HasColumnName("failure_reason");
            entity.Property(e => e.CompletedAt)
                .HasColumnType("timestamp with time zone")
                .HasColumnName("completed_at");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp with time zone")
                .HasColumnName("updated_at");
        });
        
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
} 