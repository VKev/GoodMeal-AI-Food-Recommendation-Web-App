using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<Guestrole> Guestroles { get; set; }

    public virtual DbSet<Guestrolemapping> Guestrolemappings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.Guestid).HasName("guest_pkey");

            entity.ToTable("guest");

            entity.HasIndex(e => e.Email, "guest_email_key").IsUnique();

            entity.Property(e => e.Guestid).HasColumnName("guestid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdat");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(15)
                .HasColumnName("phonenumber");
        });

        modelBuilder.Entity<Guestrole>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("guestrole_pkey");

            entity.ToTable("guestrole");

            entity.HasIndex(e => e.Rolename, "guestrole_rolename_key").IsUnique();

            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Guestrolemapping>(entity =>
        {
            entity.HasKey(e => new { e.Guestid, e.Roleid }).HasName("guestrolemapping_pkey");

            entity.ToTable("guestrolemapping");

            entity.Property(e => e.Guestid).HasColumnName("guestid");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Assignedat)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("assignedat");

            entity.HasOne(d => d.Guest).WithMany(p => p.Guestrolemappings)
                .HasForeignKey(d => d.Guestid)
                .HasConstraintName("guestrolemapping_guestid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Guestrolemappings)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("guestrolemapping_roleid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
