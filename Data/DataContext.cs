using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using TodoWebApi.Models;

namespace TodoWebApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Owner> Owners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tasks>()
                    .HasKey(tk => new { tk.Id });
            modelBuilder.Entity<Tasks>()
                    .Property(tk => tk.Priority)
                    .HasConversion<short>();
            modelBuilder.Entity<Tasks>()
                    .Property(tk => tk.Status)
                    .HasConversion<short>();
            modelBuilder.Entity<Owner>()
                    .HasKey(ow => new { ow.Email });
        }
    }
}
