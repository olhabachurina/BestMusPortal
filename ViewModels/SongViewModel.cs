using BestMusPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace BestMusPortal.ViewModels
{
    public class SongViewModel
    {
        public int SongId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        public string Artist { get; set; }

        [Required]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        [Required]
        public string Mood { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "mp3,wav", ErrorMessage = "Please upload a valid music file.")]
        public IFormFile MusicFile { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "mp4,avi", ErrorMessage = "Please upload a valid video file.")]
        public IFormFile VideoFile { get; set; }

        public int UserId { get; set; }
    }
}