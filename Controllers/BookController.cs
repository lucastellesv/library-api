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



        [HttpGet("{BookTitle}")]
        public async Task<IActionResult> Get(string BookTitle)
        {
            try
            {
                var result = await _repo.GetBooksAsyncByName(BookTitle);
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
                    return Created($"/api/book/teste", book);
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