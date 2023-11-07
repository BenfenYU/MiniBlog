using MiniBlog.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MiniBlog.Repositories
{
    public interface IArticleRepository
    {
        Task<Article> AddOneAsync(Article article);
        Task DeleteByUserId(string id);
        Task<List<Article>> GetArticles();
        Task<Article> GetById(string id);
    }
}