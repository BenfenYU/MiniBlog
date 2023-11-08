using MiniBlog.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniBlog.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> users;

        public UserRepository(IMongoClient client)
        {
            var mongoDB = client.GetDatabase("MiniBlog");
            users = mongoDB.GetCollection<User>(User.CollectionName);
        }

        public async Task<List<User>> GetAll()
        {
            var curUsers = await users.FindAsync(_ => true);

            return curUsers.ToList();
        }

        public async Task<User> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var _))
            {
                return null;
            }

            var user = await users.FindAsync(u => u.Id == id);

            return await user.FirstOrDefaultAsync();
        }

        public async Task<User> CreateOneAsync(User user)
        {
            if (user.Id == null || !ObjectId.TryParse(user.Id, out var _))
            {
                user.Id = ObjectId.GenerateNewId().ToString();
            }

            await users.InsertOneAsync(user);

            return user;
        }

        public async Task<User> UpdateByIdAsync(User user)
        {
            //var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            //var update = Builders<User>.Filter.BitsAllSet(u => u, user);
            //users.UpdateOneAsync(filter, update);
            var res = await users.ReplaceOneAsync(u => u.Id == user.Id, user);
            if (res.MatchedCount > 0)
            {
                var newUser = await users.FindAsync(u => u.Id == user.Id);
                return await newUser.FirstOrDefaultAsync();
            }
            else
            {
                return null;
            }
        }

        public async Task DeleteById(string id)
        {
            await users.DeleteOneAsync(u => u.Id == id);
        }
    }
}
