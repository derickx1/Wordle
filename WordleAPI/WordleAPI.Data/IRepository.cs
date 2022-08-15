using WordleAPI.Model;

namespace WordleAPI.Data
{
    public interface IRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserAsync(string name);
        Task DeleteUserAsync(string name);
        Task InsertUserAsync(User user);
    }
}
