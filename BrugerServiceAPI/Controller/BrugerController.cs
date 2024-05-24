using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BrugerServiceAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrugerServiceAPI.Service;
using MongoDB.Driver;
using Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


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

            // Log IP address
            var hostName = System.Net.Dns.GetHostName();
            var ips = System.Net.Dns.GetHostAddresses(hostName);
            var _ipaddr = ips.First().MapToIPv4().ToString();
            _logger.LogInformation(1, $"XYZ Service responding from {_ipaddr}");
        }

        [HttpGet("{userID}")]
        [Authorize(Roles = "2")]
        public async Task<ActionResult<User>> GetUser(Guid userID)
        {
            _logger.LogInformation("Getting user with ID: {UserID}", userID);
            var user = await _userService.GetUser(userID);
            if (user == null)
            {
                _logger.LogWarning("User with ID: {UserID} not found", userID);
                return NotFound();
            }
            return user;
        }

        [HttpGet]
        [Authorize(Roles = "2")]
        public async Task<ActionResult<IEnumerable<User>>> GetUserList()
        {
            _logger.LogInformation("Getting all users");
            var userList = await _userService.GetUserList();
            if (userList == null)
            {
                _logger.LogWarning("User list is null");
                throw new ApplicationException("User list is null");
            }
            return Ok(userList);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> AddUser(User user)
        {
            _logger.LogInformation("Adding new user: {@User}", user);
            var userID = await _userService.AddUser(user);
            _logger.LogInformation("User added with ID: {UserID}", userID);
            return Ok(userID);
        }

        [HttpPut("{_id}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> UpdateUser(Guid _id, User user)
        {
            _logger.LogInformation("Updating user with ID: {UserID}", _id);
            if (_id != user._id)
            {
                _logger.LogError("User ID in URL: {UserID} does not match ID in body: {UserIDFromBody}", _id, user._id);
                return BadRequest();
            }

            var result = await _userService.UpdateUser(user);
            if (result == 0)
            {
                _logger.LogWarning("User with ID: {UserID} not found", _id);
                return NotFound();
            }

            _logger.LogInformation("User with ID: {UserID} updated", _id);
            return Ok("User updated successfully");
        }

        [HttpDelete("{user_id}")]
        [Authorize(Roles = "2")]
        public async Task<IActionResult> DeleteUser(Guid user_id)
        {
            _logger.LogInformation("Deleting user with ID: {UserID}", user_id);
            var result = await _userService.DeleteUser(user_id);
            if (result == 0)
            {
                _logger.LogWarning("User with ID: {UserID} not found", user_id);
                return NotFound();
            }

            _logger.LogInformation("User with ID: {UserID} deleted", user_id);
            return Ok("user deleted successfully");
        }

        [HttpPost("validate")]
        public async Task<ActionResult<User>> ValidateUser([FromBody] LoginDTO user)
        {
            _logger.LogInformation("Validating user with username: {Username}, password: {Password}", user.username, user.password);
            var usr = await _userService.ValidateUser(user.username, user.password);

            if (usr == null)
            {
                _logger.LogWarning("User with username: {Username}, password: {Password}", user.username, user.password);
                return NotFound();
            }
            return Ok(usr);  // Returner brugeren som en Ok (200) svar
        }
    }
}
