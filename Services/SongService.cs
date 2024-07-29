using BestMusPortal.Data;
using BestMusPortal.Models;
using BestMusPortal.Repositorys;
using Microsoft.EntityFrameworkCore;
namespace BestMusPortal.Services
{
    public class SongService : ISongService
    {
        private readonly ISongRepository _songRepository;

        public SongService(ISongRepository songRepository)
        {
            _songRepository = songRepository;
        }

        public async Task AddSongAsync(Song song)
        {
            if (song == null) throw new ArgumentNullException(nameof(song));
            await _songRepository.AddSongAsync(song);
        }

        public async Task<IEnumerable<Song>> GetAllSongsAsync()
        {
            return await _songRepository.GetAllSongsAsync();
        }

        public async Task<Song> GetSongByIdAsync(int songId)
        {
            return await _songRepository.GetSongByIdAsync(songId);
        }

        public async Task UpdateSongAsync(Song song)
        {
            if (song == null) throw new ArgumentNullException(nameof(song));
            await _songRepository.UpdateSongAsync(song);
        }

        public async Task DeleteSongAsync(int songId)
        {
            await _songRepository.DeleteSongAsync(songId);
        }
    }
}
