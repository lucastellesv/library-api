using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_API.Models
{
    public class ResetCode
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int Code { get; set; }
    }
}
