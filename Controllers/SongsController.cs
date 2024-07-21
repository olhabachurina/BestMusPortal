using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BestMusPortal.Data;
using BestMusPortal.Models;
using BestMusPortal.Services;

namespace BestMusPortal.Controllers
{
    public class SongsController : Controller
    {
        private readonly ISongService _songService;
        private readonly IGenreService _genreService;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public SongsController(ISongService songService, IGenreService genreService, IWebHostEnvironment hostingEnvironment)
        {
            _songService = songService;
            _genreService = genreService;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var songs = await _songService.GetAllSongsAsync();
            return View(songs);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.GenreId = new SelectList(await _genreService.GetAllGenresAsync(), "GenreId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SongId,Title,Artist,GenreId")] Song song, IFormFile musicFile, IFormFile videoFile)
        {
            if (ModelState.IsValid)
            {
                if (musicFile != null && musicFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/music");
                    var uniqueMusicFileName = Guid.NewGuid().ToString() + "_" + musicFile.FileName;
                    var musicFilePath = Path.Combine(uploadsFolder, uniqueMusicFileName);
                    using (var fileStream = new FileStream(musicFilePath, FileMode.Create))
                    {
                        await musicFile.CopyToAsync(fileStream);
                    }
                    song.MusicFilePath = "/uploads/music/" + uniqueMusicFileName;
                }

                if (videoFile != null && videoFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/videos");
                    var uniqueVideoFileName = Guid.NewGuid().ToString() + "_" + videoFile.FileName;
                    var videoFilePath = Path.Combine(uploadsFolder, uniqueVideoFileName);
                    using (var fileStream = new FileStream(videoFilePath, FileMode.Create))
                    {
                        await videoFile.CopyToAsync(fileStream);
                    }
                    song.VideoFilePath = "/uploads/videos/" + uniqueVideoFileName;
                }

                await _songService.AddSongAsync(song);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.GenreId = new SelectList(await _genreService.GetAllGenresAsync(), "GenreId", "Name", song.GenreId); // Добавьте это, чтобы передать список жанров в случае ошибки валидации
            return View(song);
        }
    }
}