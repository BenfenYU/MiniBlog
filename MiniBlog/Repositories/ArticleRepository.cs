using MiniBlog.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniBlog.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IMongoCollection<Article> articles;

        public ArticleRepository(IMongoClient client)
        {
            var mongoDB = client.GetDatabase("MiniBlog");
            articles = mongoDB.GetCollection<Article>(Article.CollectionName);
        }

        public async Task<List<Article>> GetArticles()
        {
            var curArticles = await this.articles.FindAsync(_ => true);
            return curArticles.ToList();
        }

        public async Task<Article> GetById(string id)
        {
            if (!ObjectId.TryParse(id, out var _))
            {
                return null;
            }

            var item = await this.articles.FindAsync(a => a.Id == id);
            return await item.FirstOrDefaultAsync();
        }

        public async Task<Article> AddOneAsync(Article article)
        {
            if (article.Id == null || !ObjectId.TryParse(article.Id, out var _))
            {
                article.Id = ObjectId.GenerateNewId().ToString();
            }

            await articles.InsertOneAsync(article);
            return article;
        }

        public async Task DeleteByUserId(string id)
        {
            await articles.DeleteManyAsync(x => x.UserId == id);
        }
    }
}
