using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace fs.Model;

public partial class TestContext : DbContext
{
    public TestContext()
    {
    }

    public TestContext(DbContextOptions<TestContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;user id=m;database=test", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.7.7-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("latin1_swedish_ci")
            .HasCharSet("latin1");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity
                .ToTable("users")
                .HasCharSet("utf8mb4")
                .UseCollation("utf8mb4_general_ci");

            entity.Property(e => e.Id)
                .HasColumnType("int(11)")
                .HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(60)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(30)
                .HasColumnName("fullname");
            entity.Property(e => e.Nickname)
                .HasMaxLength(20)
                .HasColumnName("nickname");
            entity.Property(e => e.Password)
                .HasMaxLength(20)
                .HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
        }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
