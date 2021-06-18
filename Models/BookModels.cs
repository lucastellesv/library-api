using Library_API.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library_API.Models
{
    public class BookModels
    {
        
        public class BookFIlterModel
        {
            public string Title { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
         }
        
        public class BookViewModel
        {
            public BookViewModel(IQueryable<Book> Books, Pager Pager)
            {
                this.Pager = Pager;
                this.Books = Books.Skip((this.Pager.CurrentPage - 1) * this.Pager.PageSize).Take(this.Pager.PageSize).Include(x => x.Images)
                .ToList();
            }
            public List<Book> Books;
            public Pager Pager { get; set; }
        }
      }
}
