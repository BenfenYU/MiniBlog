using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using MiniBlog;
using MiniBlog.Model;
using MiniBlog.Repositories;
using MiniBlog.Stores;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace MiniBlogTest.ControllerTest
{
    [Collection("IntegrationTest")]
    public class ArticleControllerTest : TestBase
    {
        public ArticleControllerTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)
        {
        }

        [Fact]
        public async void Should_get_all_Article()
        {
            var mock = new Mock<IArticleRepository>();
            mock.Setup(repository => repository.GetArticles()).Returns(Task.FromResult(new List<Article>
            {
                new Article(null, "Happy new year", "Happy 2021 new year"),
                new Article(null, "Happy Halloween", "Halloween is coming"),
            }));
            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()), mock.Object);
            var response = await client.GetAsync("/article");
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<Article>>(body);
            Assert.Equal(2, users.Count);

            //var client = GetClient(new ArticleStore(), new UserStore(new List<User>()));
            //var response = await client.GetAsync("/article");
            //response.EnsureSuccessStatusCode();
            //var body = await response.Content.ReadAsStringAsync();
            //var users = JsonConvert.DeserializeObject<List<Article>>(body);
            //Assert.Equal(2, users.Count);
        }

        [Fact]
        public async void Should_create_article_fail_when_ArticleRepository_unavailable()
        {
            var client = GetClient();
            string userNameWhoWillAdd = "Tom";
            string articleContent = "What a good day today!";
            string articleTitle = "Good day";
            Article article = new Article(userNameWhoWillAdd, articleTitle, articleContent);

            var httpContent = JsonConvert.SerializeObject(article);
            StringContent content = new StringContent(httpContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var response = await client.PostAsync("/article", content);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async void Should_create_article_and_register_user_correct()
        {
            var testUserId = "654a19c0b505a1dd32cd88c5";
            var testUser = new User()
            {
                Id = testUserId,
                Name = "test",
                Email = "test"
            };
            var mockUser = new Mock<IUserRepository>();
            mockUser.Setup(repo => repo.GetByIdAsync(testUserId)).ReturnsAsync(testUser);
            mockUser.Setup(repo => repo.GetAll()).ReturnsAsync(new List<User>() { testUser });

            var testAId = "654a19c0b505a1dd32cd88c6";
            var testArticle = new Article()
            {
                Id = testAId,
                UserId = testUserId,
                UserName = "test",
                Content = "test",
                Title = "test",
            };
            var mockArtile = new Mock<IArticleRepository>();
            mockArtile.Setup(repository => repository.AddOneAsync(testArticle)).ReturnsAsync(testArticle);
            mockArtile.Setup(repo => repo.GetArticles()).ReturnsAsync(new List<Article> { testArticle });

            var client = GetClient(null, null, mockArtile.Object, mockUser.Object);

            var createArticleResponse = await client.PostAsJsonAsync("/article", testArticle);

            // It fail, please help
            Assert.Equal(HttpStatusCode.Created, createArticleResponse.StatusCode);

            var articleResponse = await client.GetAsync("/article");
            var articles = await articleResponse.Content.ReadFromJsonAsync<List<Article>>();
            Assert.Equal(testArticle, articles[0]);

            var userResponse = await client.GetAsync("/user");
            var users = await userResponse.Content.ReadFromJsonAsync<List<User>>();
            Assert.Equal(testUser, users[0]);
        }
    }
}
