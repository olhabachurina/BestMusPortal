using BestMusPortal.Models;
using BestMusPortal.Repositorys;
using Microsoft.EntityFrameworkCore;

namespace BestMusPortal.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        // Конструктор принимает репозиторий через внедрение зависимостей
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Возвращает всех пользователей, если нет пользователей, возвращает пустую коллекцию
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            if (users == null)
            {
                
                return new List<User>();
            }
            return users;
        }

        // Возвращает пользователя по ID
        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        // Добавляет нового пользователя
        public async Task AddUserAsync(User user)
        {
            await _userRepository.AddUserAsync(user);
        }

        // Обновляет данные пользователя
        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
        }

        // Удаляет пользователя по ID
        public async Task DeleteUserAsync(int userId)
        {
            await _userRepository.DeleteUserAsync(userId);
        }
    }
}