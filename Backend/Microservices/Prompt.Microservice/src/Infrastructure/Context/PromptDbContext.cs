using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class PromptDbContext : DbContext
{
    public PromptDbContext()
    {
    }

    public PromptDbContext(DbContextOptions<PromptDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessageRestaurant> MessageRestaurants { get; set; }

    public virtual DbSet<PromptSession> PromptSessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("messages_pkey");

            entity.ToTable("messages");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy)
                .UseCollation("C.utf8")
                .HasColumnName("deleted_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Message1)
                .HasColumnType("jsonb")
                .HasColumnName("message");
            entity.Property(e => e.PromptSessionId).HasColumnName("prompt_session_id");
            entity.Property(e => e.Sender)
                .HasColumnType("character varying")
                .HasColumnName("sender");

            entity.HasOne(d => d.PromptSession).WithMany(p => p.Messages)
                .HasForeignKey(d => d.PromptSessionId)
                .HasConstraintName("messages_prompt_session_id_fkey");
        });

        modelBuilder.Entity<MessageRestaurant>(entity =>
        {
            entity.HasKey(e => new { e.MessageId, e.RestaurantId }).HasName("message_restaurant_pkey");

            entity.ToTable("message_restaurant");

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.RestaurantId).HasColumnName("restaurant_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .UseCollation("C.utf8")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy)
                .UseCollation("C.utf8")
                .HasColumnName("deleted_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");

            entity.HasOne(d => d.Message).WithMany(p => p.MessageRestaurants)
                .HasForeignKey(d => d.MessageId)
                .HasConstraintName("message_restaurant_message_id_fkey");
        });

        modelBuilder.Entity<PromptSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("prompt_sessions_pkey");

            entity.ToTable("prompt_sessions");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("uuid_generate_v4()")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy)
                .UseCollation("C.utf8")
                .HasColumnName("created_by");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.DeletedBy)
                .UseCollation("C.utf8")
                .HasColumnName("deleted_by");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.UpdatedBy)
                .UseCollation("C.utf8")
                .HasColumnName("updated_by");
            entity.Property(e => e.UserId).HasColumnName("user_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
