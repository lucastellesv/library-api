using Library_API.Data;
using Library_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Library_API.Services
{
    public class BookService
    {
        public IConfiguration configuration { get; }
        private readonly ApplicationDbContext db;
        public BookService(IConfiguration Configuration, ApplicationDbContext context)
        {
            configuration = Configuration;
            db = context;
        }

        public List<Book> List()
        {
            return db.Books.Include(x => x.Images).ToList();
        }

        public Book GetBook(int id)
        {
            return db.Books.Where(x => x.Id == id).Include(x=> x.Images).FirstOrDefault();
        }

        public ResponseModel Add(Book book)
        {
            var Findbook = db.Books.Where(x => x.Id == book.Id).Include(x => x.Images).FirstOrDefault();

            if (book.Images != null)
            {
                foreach(var item in book.Images) { 
                int index = item.Url.IndexOf(",");

                item.Url = item.Url.Replace(item.Url.Substring(0, index + 1), "");
                byte[] bytes = Convert.FromBase64String(item.Url);

                string filePath, path = "wwwroot/Uploads/Books/" + item.Id + "/Photos";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    filePath = path + "/image_1." + item.FileExtension;
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    FileInfo[] files = di.GetFiles();
                    int imageNum;
                    if (files.Length > 0)
                    {
                        FileInfo file = files[0];

                        var file_splitted = file.Name.Split('_');

                        if (file_splitted.Length > 1)
                        {
                            imageNum = Convert.ToInt32(file_splitted[1].Split('.')[0]);
                        }
                        else
                        {
                            imageNum = 0;
                        }

                        filePath = path + "/image_" + (imageNum + 1).ToString() + "." + item.FileExtension;

                        file.Delete();
                    }
                    else
                    {
                        filePath = path + "/image_1." + item.FileExtension;
                    }
                }
                using (FileStream stream = new System.IO.FileStream(filePath, FileMode.Create))
                {
                    Stream mstream = new MemoryStream(bytes);
                    mstream.CopyTo(stream);
                    stream.Close();
                }
                index = filePath.IndexOf("Upload");
                item.Url = filePath.Substring(index);
            }
            }


            if (Findbook == null)
            {
                db.Add(book);
                db.SaveChanges();
                return new ResponseModel("Livro cadastrado com sucesso", 200);
            }
            return new ResponseModel("Este livro ja existe", 409);
        }

        public ResponseModel Delete(int id)
        {
            var FindBookId = db.Books.Where(x => x.Id == id).Include(x=> x.Images).FirstOrDefault();
            if (FindBookId != null)
            {
                db.Remove(FindBookId);
                db.SaveChanges();
                return new ResponseModel("Livro excluido com sucesso", 200);
            }
            return new ResponseModel("O livro não existe", 404);
        }

    }
}
