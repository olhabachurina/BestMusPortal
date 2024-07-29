using System.ComponentModel.DataAnnotations;

namespace BestMusPortal.Models
{
    public class Song
    {
        public int SongId { get; set; }
        [Required(ErrorMessage = "The Title field is required.")]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required(ErrorMessage = "The Artist field is required.")]
        [MaxLength(255)]
        public string Artist { get; set; }

        [Required(ErrorMessage = "The Genre field is required.")]
        public int GenreId { get; set; }
        public string  Genre { get; set; }

        public string Mood { get; set; }

        [Required(ErrorMessage = "The Music file is required.")]
        public string MusicFilePath { get; set; }

        [Required(ErrorMessage = "The Video file is required.")]
        public string VideoFilePath { get; set; }

        public int UserId { get; set; }
        public string VideoUrl { get; set; }
    }
}
    
    


