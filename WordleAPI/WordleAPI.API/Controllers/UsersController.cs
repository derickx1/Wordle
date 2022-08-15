using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WordleAPI.Data;
using WordleAPI.Model;

namespace WordleAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IRepository _repo;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IRepository repo, ILogger<UsersController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            IEnumerable<User> users;
            try
            {
                users = await _repo.GetAllUsersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }

            return users.ToList();
        }
        [HttpGet("~/api/Users/{name?}")]
        public async Task<ActionResult<User>> GetUser([FromRoute] string name)
        {
            User user;

            _logger.LogInformation("Executing GetUserController()");

            try
            {
                user = await _repo.GetUserAsync(name);
                if (user.guessNums == null)
                {
                    return StatusCode(503);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {

            _logger.LogInformation("Executing CreateUser()");

            try
            {
                await _repo.InsertUserAsync(user);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        [HttpPut]
        public async Task<ActionResult<User>> UpdateUser(User user)
        {

            _logger.LogInformation("Executing UpdateUser()");

            try
            {
                await _repo.InsertUserAsync(user);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }

        [HttpDelete("~/api/Users/{name?}")]
        public async Task<ActionResult<User>> DeleteUser([FromRoute] string name)
        {
            _logger.LogInformation("Executing DeleteUser()");

            try
            {
                await _repo.DeleteUserAsync(name);
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(500);
            }
        }
    }
}
