using System.ComponentModel.DataAnnotations;

namespace BestMusPortal.Models
{
    public class Genre
    {
        public int GenreId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public List<Song> Songs { get; set; }
    }
}
