using MiniBlog.Model;
using MiniBlog.Repositories;
using MiniBlog.Stores;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniBlog.Services
{
    public class UserService
    {
        private readonly IUserRepository userRepository;
        private readonly IArticleRepository articleRepository;

        public UserService(IUserRepository userRepository, IArticleRepository articleRepository)
        {
            this.userRepository = userRepository;
            this.articleRepository = articleRepository; 
        }

        public async Task<User> RegisterAsync(User user)
        {
            //user.Id = ObjectId.GenerateNewId().ToString();
            if (!ObjectId.TryParse(user.Id, out var id))
            {
                return null;
            }

            var newUser = await userRepository.CreateOneAsync(user);

            return newUser;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await userRepository.GetAll();
        }

        public async Task<User> UpdateAsync(User user)
        {
            if (!ObjectId.TryParse(user.Id, out ObjectId _))
            {
                return null;
            }

            return await userRepository.UpdateByIdAsync(user);
        }

        public async Task DeleteAsync(string id) 
        {
            await userRepository.DeleteById(id);
            await articleRepository.DeleteByUserId(id);
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await userRepository.GetByIdAsync(id);
        }
    }
}
