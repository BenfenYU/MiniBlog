using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MiniBlog.Model;
using MiniBlog.Services;
using MiniBlog.Stores;

namespace MiniBlog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService userService = null!;

        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(User user)
        {
            var newUser = await this.userService.RegisterAsync(user);

            return Created(string.Empty, newUser);
        }

        [HttpGet]
        public async Task<List<User>> GetAllAsync()
        {
            return await userService.GetAllAsync();
        }

        [HttpPut]
        public async Task<User> UpdateAsync(User user)
        {
            return await userService.UpdateAsync(user);
        }

        [HttpDelete]
        public async Task DeleteAsync(string id)
        {
            await userService.DeleteAsync(id);
        }

        [HttpGet("{id}")]
        public async Task<User> GetByIdAsync(string id)
        {
            return await userService.GetByIdAsync(id);
        }
    }
}
