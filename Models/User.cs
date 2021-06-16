using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations.Schema;
namespace Library_API.Models
{
    public class User : IdentityUser<string>
    {
        [NotMapped]
        public string Password { get; set; }

    }
    public class ForgotPasswordModel
    {
        public ForgotPasswordModel(string message, int statusCode, string userId)
        {
            Message = message;
            StatusCode = statusCode;
            UserId = userId;
        }

        public string UserId { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }

    public class ResetPasswordModel
    {
        public string UserId { get; set; }
        public string NewPassword { get; set; }
        public int Code { get; set; }
    }

    public class UpdatePasswordModel
    {
        public string Id { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}