using BestMusPortal.Models;
using BestMusPortal.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BestMusPortal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISongService _songService;

        public HomeController(ISongService songService)
        {
            _songService = songService;
        }

        public async Task<IActionResult> Index()
        {
            var songs = await _songService.GetAllSongsAsync();
            var lastSong = songs.OrderByDescending(s => s.SongId).FirstOrDefault();
            if (lastSong != null)
            {
                ViewBag.LastSongUrl = Url.Content($"~/{lastSong.VideoFilePath}");
            }

            return View();
        }
    }
}