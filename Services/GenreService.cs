using BestMusPortal.Data;
using BestMusPortal.Models;
using BestMusPortal.Repositorys;
using Microsoft.EntityFrameworkCore;
namespace BestMusPortal.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;

        public GenreService(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            return await _genreRepository.GetAllGenresAsync();
        }

        public async Task<Genre> GetGenreByIdAsync(int genreId)
        {
            return await _genreRepository.GetGenreByIdAsync(genreId);
        }

        public async Task AddGenreAsync(Genre genre)
        {
            await _genreRepository.AddGenreAsync(genre);
        }

        public async Task UpdateGenreAsync(Genre genre)
        {
            await _genreRepository.UpdateGenreAsync(genre);
        }

        public async Task DeleteGenreAsync(int genreId)
        {
            await _genreRepository.DeleteGenreAsync(genreId);
        }
    }
}