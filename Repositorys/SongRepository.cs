﻿using BestMusPortal.Data;
using BestMusPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace BestMusPortal.Repositorys
{
    public class SongRepository : ISongRepository
    {
        private readonly ApplicationDbContext _context;

        public SongRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Song>> GetAllSongsAsync()
        {
            return await _context.Songs.ToListAsync();
        }

        public async Task<Song> GetSongByIdAsync(int songId)
        {
            return await _context.Songs.FindAsync(songId);
        }

        public async Task AddSongAsync(Song song)
        {
            await _context.Songs.AddAsync(song);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSongAsync(Song song)
        {
            _context.Songs.Update(song);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSongAsync(int songId)
        {
            var song = await GetSongByIdAsync(songId);
            if (song != null)
            {
                _context.Songs.Remove(song);
                await _context.SaveChangesAsync();
            }
        }
    }
}