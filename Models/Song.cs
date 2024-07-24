using System.ComponentModel.DataAnnotations;

namespace BestMusPortal.Models
{
    public class Song
    {
        public int SongId { get; set; }
        [Required, StringLength(200)] public string Title { get; set; }
        [Required, StringLength(100)] public string Artist { get; set; }
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        [Required, StringLength(100)] public string MusicFilePath { get; set; }
        [Required, StringLength(100)] public string VideoFilePath { get; set; }
        [Required, StringLength(50)] public string Mood { get; set; }
    }
}

