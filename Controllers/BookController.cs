using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Library_API.Controllers
{ 
    [Route("api/[controller]")]
    [ApiController]
    public class BookController:Controller
    {
        public BookController()
        {
            
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
        [HttpGet("{BookId}")]
        public IActionResult Get(int BookId)
        {
            return Ok();
        }
        
                [HttpPost]
        public IActionResult Post()
        {
            return Ok();
        }
    }
}