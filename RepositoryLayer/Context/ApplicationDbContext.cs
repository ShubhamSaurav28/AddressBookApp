using System;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public virtual DbSet<UserEntity> Users { get; set; }
        public virtual DbSet<AddressBookEntity> AddressBooks { get; set; }  // Fixed Naming

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AddressBookEntity>()
                .HasOne(a => a.User)
                .WithMany(u => u.AddressBookEntries)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);  // Ensures cascading delete

            base.OnModelCreating(modelBuilder);
        }
    }
}
