﻿using BestMusPortal.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BestMusPortal.ViewModels
{
    public class SongViewModel
    {
        public int SongId { get; set; }

        [Required(ErrorMessage = "The Title field is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The Artist field is required.")]
        public string Artist { get; set; }

        [Required(ErrorMessage = "The GenreId field is required.")]
        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        [Display(Name = "Genre Name")]
        public string Genre { get; set; }
        public string Mood { get; set; }

        [Required(ErrorMessage = "The Music file is required.")]
        public IFormFile MusicFile { get; set; }

        [Required(ErrorMessage = "The Video file is required.")]
        public IFormFile VideoFile { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "The UserName field is required.")]
        public string UserName { get; set; }

        public string MusicFilePath { get; set; }
        public string VideoFilePath { get; set; }
        public string VideoUrl { get; set; }
    }

}

        