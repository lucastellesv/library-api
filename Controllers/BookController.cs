using System.Collections.Generic;
using System.Threading.Tasks;
using Library_API.Data;
using Library_API.Models;
using Library_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Library_API.Models.BookModels;

namespace Library_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : Controller
    {
        public BookService _context;
        public BookController(BookService context)
        {
            _context = context;
        }

        [HttpGet]
        public  IActionResult Get()
        {
            try
            {
                var result =  _context.List();
                return Ok(result);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "O Banco de dados falhou");
            }
        }

        [HttpPost]
        [Route("list")]
        public IActionResult List(BookFIlterModel filter)
        {
            try
            {
                var bookList = _context.List(filter);
                var teste = new
                {
                    books = bookList.Books,
                    pager = bookList.Pager
                };

                return Ok(teste);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Ocorreu um erro");
            }
        }


        [HttpGet("{BookId}")]
        public IActionResult Get(int BookId)
        {
            try
            {
                var result = _context.GetBook(BookId);
                return Ok(result);
            }
            catch (System.Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "O Banco de dados falhou");
            }

        }

        [HttpPost]
        public IActionResult Post([FromBody] Book book)
        {
            try
            {
                ResponseModel model = _context.Add(book);
                switch (model.StatusCode)
                {
                    case 200: return Ok(model);
                    default: return Conflict(model);
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
                ResponseModel model = _context.Delete(BookId);
                switch (model.StatusCode)
                {
                    case 200: return Ok(model);
                    default: return NotFound(model);
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