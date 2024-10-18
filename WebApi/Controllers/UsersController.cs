using BusinessLogic.System.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViewModels.System.Users;

namespace WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/Users/authenticate
        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Authenticate(request);
            if (string.IsNullOrEmpty(result.Data))
                return BadRequest(result);

            return Ok(result);
        }

        // POST: api/Users
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Register(request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // GET: /api/Users
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers([FromQuery] GetUserPagingRequest request)
        {
            var user = await _userService.GetAllPaged(request);
            return Ok(user);
        }

        // GET: /api/Users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id)
        {
            var result = await _userService.GetById(id);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        // GET: /api/Users/admin
        [HttpGet("get-by-name/{username}")]
        [Authorize]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var result = await _userService.GetByUsername(username);
            if (!result.IsSuccess)
                return NotFound(result);

            return Ok(result);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UserUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Update(id, request);
            if (!result.IsSuccess)
                return BadRequest(result);

            return Ok(result);
        }

        // PUT: api/Users/5/roles-assign
        [HttpPut("{id}/roles-assign")]
        [Authorize]
        public async Task<IActionResult> RoleAssign([FromRoute] Guid id, [FromBody] RoleAssignRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.RoleAssign(id, request);
            if (!result.IsSuccess)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            await _userService.Delete(id);
            return NoContent();
        }
    }
}