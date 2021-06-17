using System.ComponentModel.DataAnnotations;
namespace Library_API.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Url is needed.")]
        public string Url { get; set; }
        public string FileExtension { get; set; }
    }
}