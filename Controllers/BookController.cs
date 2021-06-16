using System.Threading.Tasks;
using Library_API.Data;
using Library_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;



namespace Library_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : Controller
    {
        public IRepository _repo { get; }
        public BookController(IRepository repo)
        {
            _repo = repo;

        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await _repo.GetAllBooksAsync();
                return Ok(result);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "O Banco de dados falhou");
            }
        }



        [HttpGet("{BookId}")]
        public async Task<IActionResult> Get(int BookId)
        {
            try
            {
                var result = await _repo.GetBooksAsyncById(BookId);
                return Ok(result);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "O Banco de dados falhou");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Book book)
        {
            try
            {

                _repo.Add(book);
                if (await _repo.SaveChangesAsync())
                {
                    return Created($"/api/book/", book);
                }
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "O Banco de dados falhou");
            }
            return BadRequest();
        }

        [HttpDelete("{BookId}")]
       public async Task<IActionResult> Delete(int BookId)
         {
            try
             {
                 var book = await _repo.GetBooksAsyncById(BookId);
                 if(book == null) return NotFound();

                 _repo.Delete(book);

                 if(await _repo.SaveChangesAsync())
                 {
                     return Ok();
                 }
            }
             catch (System.Exception)
             {
                 return this.StatusCode(StatusCodes.Status500InternalServerError, "O Banco de dados falhou");
             }
             return BadRequest();
         }

         [HttpPut("{BookId}")]
       public async Task<IActionResult> Put(int BookId)
         {
            try
             {
                 var book = await _repo.GetBooksAsyncById(BookId);
                 if(book == null) return NotFound();

                 _repo.Update(book);

                 if(await _repo.SaveChangesAsync())
                 {
                     return Ok();
                 }
            }
             catch (System.Exception)
             {
                 return this.StatusCode(StatusCodes.Status500InternalServerError, "O Banco de dados falhou");
             }
             return BadRequest();
         }
    }
}