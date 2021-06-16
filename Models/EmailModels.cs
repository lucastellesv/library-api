using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Library_API.Models
{
    public class EmailModels
    {
        public class EmailModel
        {
            public EmailModel(string name, string to, string subject, string message)
            {
                this.Name = name;
                this.To = to;
                this.Subject = subject;
                this.Message = message;
            }

            [Required, Display(Name = "Email de destino"), EmailAddress]
            public string To { get; set; }

            [Required, Display(Name = "Assunto")]
            public string Subject { get; set; }

            [Required, Display(Name = "Mensagem")]
            public string Message { get; set; }

            public string Name { get; set; }
        }
    }
}
