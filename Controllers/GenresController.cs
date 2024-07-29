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
using Microsoft.AspNetCore.Authorization;
using BestMusPortal.ViewModels;

namespace BestMusPortal.Controllers
{
    [Authorize]
    public class GenresController : Controller
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
        {
            var genres = await _genreService.GetAllGenresAsync();
            var genreViewModels = genres.Select(g => new GenreViewModel
            {
                GenreId = g.GenreId,
                Name = g.Name
            });
            return View(genreViewModels);
        }
       
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] GenreViewModel genreViewModel)
        {
            if (ModelState.IsValid)
            {
                var genre = new Genre
                {
                    Name = genreViewModel.Name
                };
                await _genreService.AddGenreAsync(genre);
                return RedirectToAction(nameof(Index));
            }
            return View(genreViewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            var genreViewModel = new GenreViewModel
            {
                GenreId = genre.GenreId,
                Name = genre.Name
            };

            return View(genreViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GenreId,Name")] GenreViewModel genreViewModel)
        {
            if (id != genreViewModel.GenreId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var genre = new Genre
                {
                    GenreId = genreViewModel.GenreId,
                    Name = genreViewModel.Name
                };

                try
                {
                    await _genreService.UpdateGenreAsync(genre);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await GenreExists(genre.GenreId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(genreViewModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
            {
                return NotFound();
            }

            var genreViewModel = new GenreViewModel
            {
                GenreId = genre.GenreId,
                Name = genre.Name
            };

            return View(genreViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _genreService.DeleteGenreAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> GenreExists(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            return genre != null;
        }
    }
}