using System.ComponentModel.DataAnnotations;

namespace BestMusPortal.ViewModels
{
    public class GenreViewModel
    {
        public int GenreId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
