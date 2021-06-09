using System.Threading.Tasks;
using Library_API.Models;

namespace Library_API.Data
{
    public interface IRepository
    {
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        Task<bool> SaveChangesAsync();

        Task<Book[]> GetAllBooksAsync();

        Task<Book[]> GetBooksAsyncByName(string BookTitle);

    }
}