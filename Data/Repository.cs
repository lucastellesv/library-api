using System.Linq;
using System.Threading.Tasks;
using Library_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Data
{
    public class Repository : IRepository
    {
        public ApplicationDbContext _context { get; }

        public Repository(ApplicationDbContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        public void Update<T>(T entity) where T : class
        {
            _context.Update(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }

        //Metodos Get Book
        public async Task<Book[]> GetAllBooksAsync()
        {
            IQueryable<Book> query = _context.Books;

            query = query.AsNoTracking().OrderBy(b => b.Id);

            return await query.ToArrayAsync();
        }

        public async Task<Book[]> GetBooksAsyncByName(string BookTitle)
        {
            IQueryable<Book> query = _context.Books;

            query = query.AsNoTracking().OrderBy(b => b.Title).Where(book => book.Title == BookTitle);

            return await query.ToArrayAsync();
        }
    }
}