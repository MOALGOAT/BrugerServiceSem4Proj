using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrugerServiceAPI.Models;

namespace BrugerServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserInterface _userService;
        private readonly ILogger<UserController> _logger; 

        public UserController(IUserInterface userService, ILogger<UserController> logger) 
        {
            _userService = userService;
            _logger = logger; 
        }

        [HttpGet("{_id}")]
        public async Task<ActionResult<User>> GetUser(Guid userID)
        {
            var user = await _userService.GetUser(userID);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUserList()
        {
            var userList = await _userService.GetUserList();
            if (userList == null) { throw new ApplicationException("listen er null"); };
            return Ok(userList);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> AddUser(User user)
        {
            var userID = await _userService.AddUser(user);
            return CreatedAtAction(nameof(GetUser), new { b = userID }, userID);
        }

        [HttpPut("{_id}")]
        public async Task<IActionResult> UpdateUser(Guid id, User user)
        {
            if (id != user._id)
            {
                return BadRequest();
            }

            var result = await _userService.UpdateUser(user);
            if (result == 0)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{user_id}")]
        public async Task<IActionResult> DeleteUser(Guid user_id)
        {
            var result = await _userService.DeleteUser(user_id);
            if (result == 0)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
