using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace rpmtwo.Models;

public partial class AeroflotDbContext : DbContext
{
    public AeroflotDbContext()
    {
    }

    public AeroflotDbContext(DbContextOptions<AeroflotDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Рейсы> Рейсы { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost\\sqlexpress;Database=AeroflotDB;Trusted_Connection=True;Encrypt=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Рейсы>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Рейсы__3214EC27DF6F4320");

            entity.ToTable("Рейсы");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ВремяВылета).HasColumnType("datetime");
            entity.Property(e => e.ВремяПрибытия).HasColumnType("datetime");
            entity.Property(e => e.НомерРейса).HasMaxLength(10);
            entity.Property(e => e.ПунктНазначения).HasMaxLength(100);
            entity.Property(e => e.ТипСамолета).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
