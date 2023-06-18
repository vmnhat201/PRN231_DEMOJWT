using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JWTDemo.Models
{
    public partial class DemoJWTContext : DbContext
    {
        public DemoJWTContext()
        {
        }

        public DemoJWTContext(DbContextOptions<DemoJWTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =(local); database=DemoJWT;uid=sa;pwd=sa;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("User");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.Password)
                    .HasMaxLength(40)
                    .IsFixedLength();

                entity.Property(e => e.Role)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.UserName)
                    .HasMaxLength(40)
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
