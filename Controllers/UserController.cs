using Library_API.Data;
using Library_API.Models;
using Library_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Library_API.Controllers
{
    [Authorize("Bearer")]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext db;
        private readonly UserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signManager;
        private readonly IHttpClientFactory _clientFactory;
        public IConfiguration configuration { get; }
        public UserController(IConfiguration Configuration, ApplicationDbContext context, UserManager<User> userManager,
            SignInManager<User> signinManager, IHttpClientFactory clientFactory, UserService userService)
        {
            _clientFactory = clientFactory;
            configuration = Configuration;
            db = context;
            _userService = userService;
            _signManager = signinManager;
            _userManager = userManager;
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(User usr)
        {
            RegisterModel retModel = await _userService.Register(usr);
            object retObj = new { message = retModel.Message };
            switch (retModel.StatusCode)
            {
                case 200:
                    return Ok(retObj);
                case 500:
                    return StatusCode(500, retObj);
                case 422:
                    return UnprocessableEntity(retObj);
                case 409:
                    return Conflict(retObj);
            }
            return null;
        }

        [HttpPost]
        [Route("token")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody] User usr)
        {
            try
            {
                GetTokenModel response = await _userService.GetToken(usr);
                if (response.StatusCode == 200)
                {
                    return Ok(response);
                }
                else
                {
                    return Unauthorized(response);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
