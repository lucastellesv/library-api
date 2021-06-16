using Library_API.Data;
using Library_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Library_API.Models.EmailModels;

namespace Library_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [AllowAnonymous]
    public class EmailController : ControllerBase
    {
        private readonly ApplicationDbContext db;
        private readonly EmailService _emailService;

        private readonly IViewRenderService _viewRenderService;
        public IConfiguration configuration { get; }

        public EmailController(IConfiguration Configuration, ApplicationDbContext context, EmailService emailService, IViewRenderService viewRenderService)
        {
            configuration = Configuration;
            db = context;
            _emailService = emailService;
            _viewRenderService = viewRenderService;
        }

        [HttpPost]
        [Route("send")]
        [AllowAnonymous]
        public IActionResult SendEmailAsync(EmailModel model)
        {
            try
            {
                var viewResult = _viewRenderService.RenderToStringAsync("Email/EmailTemplate", new EmailModel(
                model.Name,
                model.To,
                model.Subject,
                model.Message
                )).Result;

                return  Ok(_emailService.SendEmailAsync(model.To, model.Subject, viewResult));
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Ocorreu um erro");
            }
        }

    }
}
