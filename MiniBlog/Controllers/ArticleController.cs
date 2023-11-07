using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MiniBlog.Model;
using MiniBlog.Services;
using MiniBlog.Stores;

namespace MiniBlog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly ArticleService articleService = null!;

        public ArticleController(ArticleService articleService)
        {
            this.articleService = articleService;
        }

        [HttpGet]
        public async Task<List<Article>> ListAsync()
        {
            return await this.articleService.GetAllAsync();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Article article)
        {
            var added = await this.articleService.CreateArticleAsync(article);
            return Created(string.Empty, added);
        }

        [HttpGet("{id}")]
        public async Task<Article> GetByIdAsync(string id)
        {
            return await this.articleService.GetByIdAsync(id);
        }
    }
}
