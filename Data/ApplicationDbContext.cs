using System.Collections.Generic;
using Library_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().Property(e => e.Id).ValueGeneratedOnAdd();
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options){ }
        public DbSet<Book> Books {get; set;}
        public DbSet<User> Users { get; set; }
        public DbSet<ResetCode> ResetCodes { get; set; }

    }
}