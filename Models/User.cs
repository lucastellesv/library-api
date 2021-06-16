using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Library_API.Models
{
    public class User : IdentityUser<string>
    {      
        [NotMapped]
        public string Password { get; set; }
        
    }

}