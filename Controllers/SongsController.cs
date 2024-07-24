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
using BestMusPortal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

namespace BestMusPortal.Controllers
{

    [Authorize]
   
    public class SongsController : Controller
    {
        private readonly ISongService _songService;
        private readonly IGenreService _genreService;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<SongsController> _logger;

        public SongsController(ISongService songService, IGenreService genreService, IUserService userService, IWebHostEnvironment webHostEnvironment, ILogger<SongsController> logger)
        {
            _songService = songService;
            _genreService = genreService;
            _userService = userService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("GET Create action started.");
            ViewBag.GenreId = new SelectList(await _genreService.GetAllGenresAsync(), "GenreId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SongViewModel songViewModel)
        {
            _logger.LogInformation("POST Create action started.");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Model state is invalid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError(error.ErrorMessage);
                }
                ViewBag.GenreId = new SelectList(await _genreService.GetAllGenresAsync(), "GenreId", "Name", songViewModel.GenreId);
                return View(songViewModel);
            }

            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                _logger.LogError("User is not authenticated.");
                return BadRequest("User is not authenticated");
            }

            var user = await _userService.GetUserByNameAsync(userName);
            if (user == null)
            {
                _logger.LogError("User not found.");
                return NotFound("User not found");
            }

            songViewModel.UserId = user.UserId;

            var musicFilePath = await SaveFileAsync(songViewModel.MusicFile, "uploads/music");
            var videoFilePath = await SaveFileAsync(songViewModel.VideoFile, "uploads/videos");

            if (musicFilePath == null || videoFilePath == null)
            {
                _logger.LogError("Failed to save one or both files.");
                ModelState.AddModelError(string.Empty, "Failed to save one or both files.");
                ViewBag.GenreId = new SelectList(await _genreService.GetAllGenresAsync(), "GenreId", "Name", songViewModel.GenreId);
                return View(songViewModel);
            }

            var song = new Song
            {
                Title = songViewModel.Title,
                Artist = songViewModel.Artist,
                GenreId = songViewModel.GenreId,
                UserId = songViewModel.UserId,
                MusicFilePath = musicFilePath,
                VideoFilePath = videoFilePath,
                Mood = songViewModel.Mood
            };

            try
            {
                _logger.LogInformation($"Saving song: {song.Title}, Artist: {song.Artist}, GenreId: {song.GenreId}, UserId: {song.UserId}");
                await _songService.AddSongAsync(song);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding song.");
                ModelState.AddModelError(string.Empty, "An error occurred while adding the song. Please try again.");
                ViewBag.GenreId = new SelectList(await _genreService.GetAllGenresAsync(), "GenreId", "Name", songViewModel.GenreId);
                return View(songViewModel);
            }
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folderPath)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogError("File is null or empty.");
                return null;
            }

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                _logger.LogInformation($"Saving file to: {filePath}");
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                return "/" + folderPath + "/" + uniqueFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving file.");
                return null;
            }
        }
    }
}