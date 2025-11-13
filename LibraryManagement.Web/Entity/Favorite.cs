using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagement.Web.Entity
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }   

        [Required]
        public int BookId { get; set; }

        
        [ForeignKey("BookId")]
        public Book Book { get; set; }
    }
}
