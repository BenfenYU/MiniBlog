using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Runtime.CompilerServices;
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
    public class UserControllerTest : TestBase
    {
        public UserControllerTest(CustomWebApplicationFactory<Startup> factory)
            : base(factory)

        {
        }

        [Fact]
        public async Task Should_get_all_users()
        {
            var mock = new Mock<IUserRepository>();
            mock.Setup(repository => repository.GetAll()).ReturnsAsync(new List<User>());
            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()), null, mock.Object);
            var response = await client.GetAsync("/user");
            var users = await response.Content.ReadFromJsonAsync<List<User>>();
            Assert.Equal(0, users.Count);
        }

        [Fact]
        public async Task Should_register_user_success()
        {
            var mock = new Mock<IUserRepository>();
            var user = new User()
            {
                Id = "654a19c0b505a1dd32cd88c5",
                Name = "Test",
                Email = "test",
            };
            mock.Setup(repository => repository.CreateOneAsync(user)).ReturnsAsync(user);

            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()), null, mock.Object);
            var response = await client.PostAsJsonAsync("/user", user);
            var newUser = await response.Content.ReadFromJsonAsync<User>();
            Assert.Equal(user, newUser);
        }

        [Fact]
        public async Task Should_register_user_fail_when_UserStore_unavailable()
        {
            var client = GetClient();

            var userName = "Tom";
            var email = "a@b.com";
            var user = new User(userName, email) { Id = "654a19c0b505a1dd32cd88c5" };
            var userJson = JsonConvert.SerializeObject(user);
            StringContent content = new StringContent(userJson, Encoding.UTF8, MediaTypeNames.Application.Json);
            var registerResponse = await client.PostAsync("/user", content);
            Assert.Equal(HttpStatusCode.InternalServerError, registerResponse.StatusCode);
        }

        [Fact]
        public async Task Should_update_user_email_success_()
        {
            var mock = new Mock<IUserRepository>();
            var user = new User()
            {
                Id = "654a19c0b505a1dd32cd88c5",
                Name = "Test",
                Email = "test",
            };
            var newEmail = "6";
            var newUser = new User()
            {
                Id = "654a19c0b505a1dd32cd88c5",
                Name = "Test",
                Email = newEmail,
            };
            mock.Setup(repository => repository.CreateOneAsync(user)).ReturnsAsync(user);
            mock.Setup(repository => repository.UpdateByIdAsync(newUser)).ReturnsAsync(newUser);

            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()), null, mock.Object);

            var registerResponse = await client.PostAsJsonAsync("/user", user);

            var res = await client.PutAsJsonAsync("/user", newUser);
            var resE = await res.Content.ReadFromJsonAsync<User>();

            Assert.Equal(newEmail, resE.Email);
        }

        [Fact]
        public async Task Should_delete_user_and_related_article_success()
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
            mockUser.Setup(repository => repository.DeleteById(testUserId));
            mockUser.Setup(repo => repo.GetAll()).ReturnsAsync(new List<User>());

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
            mockArtile.Setup(repository => repository.DeleteByUserId(testUserId));
            mockArtile.Setup(repo => repo.GetArticles()).ReturnsAsync(new List<Article>());

            var client = GetClient(new ArticleStore(), new UserStore(new List<User>()), mockArtile.Object, mockUser.Object);

            await client.PostAsJsonAsync("/article", testArticle);
            await client.DeleteAsync($"/user/{testUserId}");

            var articlesAfterDeleteUser = await GetArticles(client);
            Assert.Equal(0, articlesAfterDeleteUser.Count);

            var usersAfterDeleteUser = await GetUsers(client);
            Assert.Equal(0, usersAfterDeleteUser.Count);
        }

        private static async Task<List<User>> GetUsers(HttpClient client)
        {
            var response = await client.GetAsync("/user");
            var body = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(body);
            return users;
        }

        private static async Task<List<Article>> GetArticles(HttpClient client)
        {
            var articleResponse = await client.GetAsync("/article");
            var articlesJson = await articleResponse.Content.ReadAsStringAsync();
            var articles = JsonConvert.DeserializeObject<List<Article>>(articlesJson);
            return articles;
        }

        private static async Task PrepareArticle(Article article1, HttpClient client)
        {
            StringContent registerUserContent = new StringContent(JsonConvert.SerializeObject(article1), Encoding.UTF8, MediaTypeNames.Application.Json);
            await client.PostAsync("/article", registerUserContent);
        }
    }
}
