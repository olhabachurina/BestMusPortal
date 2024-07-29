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
using Newtonsoft.Json;
using System.Diagnostics;
namespace BestMusPortal.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
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

        public async Task<IActionResult> Index()
        {
            var songs = await _songService.GetAllSongsAsync();
            var songViewModels = songs.Select(song => new SongViewModel
            {
                SongId = song.SongId,
                Title = song.Title,
                Artist = song.Artist,
                GenreId = song.GenreId,
                Genre = song.Genre,
                Mood = song.Mood,
                MusicFilePath = song.MusicFilePath,
                VideoFilePath = song.VideoFilePath,
                UserId = song.UserId,
                VideoUrl = song.VideoUrl
            }).ToList();

            var lastSong = songs.OrderByDescending(s => s.SongId).FirstOrDefault();
            if (lastSong != null)
            {
                ViewBag.LastSongUrl = Url.Content($"~/{lastSong.VideoFilePath}");
            }
            return View(songViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Starting Create (GET) method.");

            try
            {
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

                var genres = await _genreService.GetAllGenresAsync();
                ViewBag.Genres = new SelectList(genres, "GenreId", "Name");

                ViewBag.MoodList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Happy", Text = "Happy" },
                new SelectListItem { Value = "Sad", Text = "Sad" },
                new SelectListItem { Value = "Relaxed", Text = "Relaxed" },
                new SelectListItem { Value = "Energetic", Text = "Energetic" }
            };

                var model = new SongViewModel
                {
                    UserId = user.UserId,
                    UserName = user.UserName
                };

                _logger.LogInformation("Ending Create (GET) method.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Create (GET) method.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SongViewModel songViewModel)
        {
            _logger.LogInformation("Starting Create (POST) method.");
            _logger.LogInformation("GenreId: {GenreId}, UserName: {UserName}, MusicFile: {MusicFile}, VideoFile: {VideoFile}", songViewModel.GenreId, songViewModel.UserName, songViewModel.MusicFile, songViewModel.VideoFile);

            try
            {
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

                string musicFilePath = null;
                string videoFilePath = null;

                // Сохранение музыкального файла
                if (songViewModel.MusicFile != null)
                {
                    var musicDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/music");
                    if (!Directory.Exists(musicDir))
                    {
                        Directory.CreateDirectory(musicDir);
                    }

                    musicFilePath = Path.Combine("uploads/music", $"{Guid.NewGuid()}{Path.GetExtension(songViewModel.MusicFile.FileName)}");
                    var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, musicFilePath);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await songViewModel.MusicFile.CopyToAsync(stream);
                    }
                }

                // Сохранение видеофайла
                if (songViewModel.VideoFile != null)
                {
                    var videoDir = Path.Combine(_webHostEnvironment.WebRootPath, "uploads/videos");
                    if (!Directory.Exists(videoDir))
                    {
                        Directory.CreateDirectory(videoDir);
                    }

                    videoFilePath = Path.Combine("uploads/videos", $"{Guid.NewGuid()}{Path.GetExtension(songViewModel.VideoFile.FileName)}");
                    var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, videoFilePath);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await songViewModel.VideoFile.CopyToAsync(stream);
                    }

                    // Устанавливаем URL для видеофайла
                    songViewModel.VideoUrl = videoFilePath;
                }

                // Установка значений в ViewModel
                songViewModel.MusicFilePath = musicFilePath;
                songViewModel.VideoFilePath = videoFilePath;
                songViewModel.UserName = user.UserName;

                // Валидация модели после установки всех необходимых значений
                ModelState.Clear();
                TryValidateModel(songViewModel);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid.");
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    foreach (var error in errors)
                    {
                        _logger.LogWarning("Model error: {Error}", error);
                    }

                    var genres = await _genreService.GetAllGenresAsync();
                    ViewBag.Genres = new SelectList(genres, "GenreId", "Name");

                    ViewBag.MoodList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Happy", Text = "Happy" },
                    new SelectListItem { Value = "Sad", Text = "Sad" },
                    new SelectListItem { Value = "Relaxed", Text = "Relaxed" },
                    new SelectListItem { Value = "Energetic", Text = "Energetic" }
                };

                    return View(songViewModel);
                }

                var genre = await _genreService.GetGenreByIdAsync(songViewModel.GenreId);
                songViewModel.Genre = genre?.Name;

                var song = new Song
                {
                    Title = songViewModel.Title,
                    Artist = songViewModel.Artist,
                    GenreId = songViewModel.GenreId,
                    Genre = songViewModel.Genre,
                    UserId = user.UserId,
                    MusicFilePath = musicFilePath,
                    VideoFilePath = videoFilePath,
                    VideoUrl = songViewModel.VideoUrl,
                    Mood = songViewModel.Mood
                };

                await _songService.AddSongAsync(song);
                TempData["Success"] = "Песня успешно создана!";
                TempData["MusicFilePath"] = musicFilePath;
                TempData["VideoFilePath"] = videoFilePath;

                _logger.LogInformation("Song created successfully. Title: {Title}, Artist: {Artist}, Genre: {Genre}", song.Title, song.Artist, song.Genre);
                return RedirectToAction(nameof(Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding song.");
                ModelState.AddModelError(string.Empty, "An error occurred while adding the song. Please try again.");

                var genres = await _genreService.GetAllGenresAsync();
                ViewBag.Genres = new SelectList(genres, "GenreId", "Name");

                ViewBag.MoodList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Happy", Text = "Happy" },
                new SelectListItem { Value = "Sad", Text = "Sad" },
                new SelectListItem { Value = "Relaxed", Text = "Relaxed" },
                new SelectListItem { Value = "Energetic", Text = "Energetic" }
            };

                return View(songViewModel);
            }
        }

        [HttpGet]
        public IActionResult Success()
        {
            _logger.LogInformation("Displaying Success page.");
            ViewBag.SuccessMessage = TempData["Success"];
            ViewBag.MusicFilePath = TempData["MusicFilePath"];
            ViewBag.VideoFilePath = TempData["VideoFilePath"];
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var song = await _songService.GetSongByIdAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            var genres = await _genreService.GetAllGenresAsync();
            ViewBag.Genres = new SelectList(genres, "GenreId", "Name");

            ViewBag.MoodList = new List<SelectListItem>
        {
            new SelectListItem { Value = "Happy", Text = "Happy" },
            new SelectListItem { Value = "Sad", Text = "Sad" },
            new SelectListItem { Value = "Relaxed", Text = "Relaxed" },
            new SelectListItem { Value = "Energetic", Text = "Energetic" }
        };

            var model = new SongViewModel
            {
                SongId = song.SongId,
                Title = song.Title,
                Artist = song.Artist,
                GenreId = song.GenreId,
                Genre = song.Genre,
                Mood = song.Mood,
                MusicFilePath = song.MusicFilePath,
                VideoFilePath = song.VideoFilePath,
                UserId = song.UserId,
                VideoUrl = song.VideoUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SongViewModel songViewModel)
        {
            if (id != songViewModel.SongId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var genres = await _genreService.GetAllGenresAsync();
                ViewBag.Genres = new SelectList(genres, "GenreId", "Name");

                ViewBag.MoodList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Happy", Text = "Happy" },
                new SelectListItem { Value = "Sad", Text = "Sad" },
                new SelectListItem { Value = "Relaxed", Text = "Relaxed" },
                new SelectListItem { Value = "Energetic", Text = "Energetic" }
            };

                return View(songViewModel);
            }

            var song = await _songService.GetSongByIdAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            song.Title = songViewModel.Title;
            song.Artist = songViewModel.Artist;
            song.GenreId = songViewModel.GenreId;
            song.Genre = songViewModel.Genre;
            song.Mood = songViewModel.Mood;
            song.MusicFilePath = songViewModel.MusicFilePath;
            song.VideoFilePath = songViewModel.VideoFilePath;
            song.VideoUrl = songViewModel.VideoUrl;

            await _songService.UpdateSongAsync(song);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var song = await _songService.GetSongByIdAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            var model = new SongViewModel
            {
                SongId = song.SongId,
                Title = song.Title,
                Artist = song.Artist,
                GenreId = song.GenreId,
                Genre = song.Genre,
                Mood = song.Mood,
                MusicFilePath = song.MusicFilePath,
                VideoFilePath = song.VideoFilePath,
                UserId = song.UserId,
                VideoUrl = song.VideoUrl
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _songService.DeleteSongAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> FilterSongs(string genreFilter, string artistFilter, string moodFilter, string titleFilter)
        {
            Trace.WriteLine($"Received filters - Genre: {genreFilter}, Artist: {artistFilter}, Mood: {moodFilter}, Title: {titleFilter}");

            var songs = await _songService.GetAllSongsAsync();

            // Логирование начального количества песен
            Trace.WriteLine($"Initial song count: {songs.Count()}");

            if (!string.IsNullOrEmpty(genreFilter) && genreFilter.ToLower() != "all")
            {
                songs = songs.Where(s => s.Genre.ToLower() == genreFilter.ToLower()).ToList();
                Trace.WriteLine($"Songs after genre filter ({genreFilter}): {songs.Count()}");
            }

            if (!string.IsNullOrEmpty(artistFilter))
            {
                Trace.WriteLine($"Artist filter before processing: {artistFilter}");
                songs = songs.Where(s => s.Artist.ToLower().Contains(artistFilter.ToLower().Trim())).ToList();
                Trace.WriteLine($"Songs after artist filter ({artistFilter}): {songs.Count()}");
                foreach (var song in songs)
                {
                    Trace.WriteLine($"Song: {song.Title}, Artist: {song.Artist}");
                }
            }

            if (!string.IsNullOrEmpty(moodFilter) && moodFilter.ToLower() != "all")
            {
                songs = songs.Where(s => s.Mood.ToLower() == moodFilter.ToLower()).ToList();
                Trace.WriteLine($"Songs after mood filter ({moodFilter}): {songs.Count()}");
            }

            if (!string.IsNullOrEmpty(titleFilter))
            {
                songs = songs.Where(s => s.Title.ToLower().Contains(titleFilter.ToLower())).ToList();
                Trace.WriteLine($"Songs after title filter ({titleFilter}): {songs.Count()}");
            }

            var songViewModels = songs.Select(song => new SongViewModel
            {
                SongId = song.SongId,
                Title = song.Title,
                Artist = song.Artist,
                GenreId = song.GenreId,
                Genre = song.Genre,
                Mood = song.Mood,
                MusicFilePath = song.MusicFilePath,
                VideoFilePath = song.VideoFilePath,
                UserId = song.UserId,
                VideoUrl = song.VideoUrl
            }).ToList();

            // Логирование конечного количества песен
            Trace.WriteLine($"Final song count: {songViewModels.Count}");

            return PartialView("_SongListPartial", songViewModels);
        }
    }
}