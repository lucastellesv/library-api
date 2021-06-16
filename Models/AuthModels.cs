using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_API.Models
{
    public class GetTokenModel
    {
        public GetTokenModel(string token, object user, DateTime validate, int statusCode, string message)
        {
            Token = token;
            User = user;
            ValidTo = validate;
            StatusCode = statusCode;
            Message = message;
        }

        public GetTokenModel(string token, object user, DateTime validate, int statusCode)
        {
            Token = token;
            User = user;
            ValidTo = validate;
            StatusCode = statusCode;
        }

        public GetTokenModel(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }

        public string Token { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public object User { get; set; }
        public DateTime ValidTo { get; set; }
    }

    public class RegisterModel
    {
        public RegisterModel(int statusCode)
        {
            StatusCode = statusCode;
        }

        public RegisterModel(string message, int statusCode)
        {
            Message = message;
            StatusCode = statusCode;
        }


        public string Message { get; set; }
        public int StatusCode { get; set; }
    }

   
}
