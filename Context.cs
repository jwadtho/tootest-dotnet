using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tootest_dotnet.Entities;

namespace tootest_dotnet
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<ProductEntity> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductEntity>().ToTable("product");
            modelBuilder.Entity<ProductEntity>().HasKey(b => b.Id);
            modelBuilder.Entity<ProductEntity>().Property(b => b.Id).HasColumnName("id");
            modelBuilder.Entity<ProductEntity>().Property(b => b.Name).HasColumnName("name");
        }
    }
}
