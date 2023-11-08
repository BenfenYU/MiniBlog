using MiniBlog.Model;
using MiniBlog.Repositories;
using MiniBlog.Stores;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniBlog.Services
{
    public class ArticleService
    {
        private readonly IArticleRepository articleRepository = null!;
        private readonly IUserRepository userRepository = null!;

        public ArticleService(IArticleRepository articleRepository, IUserRepository userRepository)
        {
            this.articleRepository = articleRepository;
            this.userRepository = userRepository;
        }

        public async Task<Article> CreateArticleAsync(Article article)
        {
            var user = await userRepository.GetByIdAsync(article.UserId);

            if (user == null)
                {
                    user = await userRepository.CreateOneAsync(new User(article.UserName));
                }

            article.UserId = user.Id;

            article = await articleRepository.AddOneAsync(article);
            
            return article;
        }

        public async Task<Article> GetByIdAsync(string id)
        {
            return await this.articleRepository.GetById(id);
        }

        public async Task<List<Article>> GetAllAsync()
        {
            return await this.articleRepository.GetArticles();
        }
    }
}
