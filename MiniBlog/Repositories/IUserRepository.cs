using MiniBlog.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniBlog.Repositories
{
    public interface IUserRepository
    {
        Task<User> CreateOneAsync(User user);
        Task DeleteById(string id);
        Task<List<User>> GetAll();
        Task<User> GetByIdAsync(string id);
        Task<User> UpdateByIdAsync(User user);
    }
}