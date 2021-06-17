using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Library_API.Models
{
    public class Book
    {


        [Key]
        public int Id { get; set; }


        [MinLength(1, ErrorMessage = "The book title needs a minimum with 1 character.")]
        [MaxLength(80, ErrorMessage = "The book title needs a maximum with 80 characters.")]
        [Required(ErrorMessage = "Title required.")]
        public string Title { get; set; }

        [MinLength(1, ErrorMessage = "The book author needs a minimum with 1 character.")]
        [MaxLength(80, ErrorMessage = "The book author needs a maximum with 80 characters.")]
        [Required(ErrorMessage = "Author is needed.")]
        public string Author { get; set; }

        [MinLength(20, ErrorMessage = "The book description needs a minimum with 20 character.")]
        [MaxLength(1200, ErrorMessage = "The book description needs a maximum with 1200 characters.")]
        [Required(ErrorMessage = "Description is needed.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Gender is needed")]
        public string Gender { get; set; }

        [MinLength(1, ErrorMessage = "The book language needs a minimum with 1 character.")]
        [MaxLength(25, ErrorMessage = "The book language needs a maximum with 25 characters.")]
        [Required(ErrorMessage = "Language is needed.")]
        public string Language { get; set; }

        [Required(ErrorMessage = "Image is needed.")]
        public List<Image> Images {get; set; }

    }
}