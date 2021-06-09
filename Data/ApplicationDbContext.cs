using System.Collections.Generic;
using Library_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base (options){ }
        public DbSet<Book> Books {get; set;}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Book>()
            .HasData(
                new List<Book>(){
                    new Book(){
                        Id = 1,
                        Title = "O nome do vento",
                        Author = "Patrick Rothfuss",
                        Description = " O Nome do Vento é um livro de fantasia escrito pelo norte-americano Patrick Rothfuss, o primeiro da série intitulada A Crônica do Matador do Rei.",
                        Gender = "Fantasia",
                        Language = "Portugues"
                    }
                }
            );
        }
    }
}