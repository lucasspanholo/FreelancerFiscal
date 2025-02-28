using Microsoft.AspNetCore.Mvc;
using FreelancerFiscal.Application.Services; // Adicionar esta diretiva
using FreelancerFiscal.Application.DTOs; // Adicionar esta diretiva

namespace FreelancerFiscal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var registeredUser = await _userService.RegisterUserAsync(registerUserDto);

                if (registeredUser == null)
                {
                    return BadRequest("User registration failed");
                }

                // Returns a 201 Created status with the newly created user details
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = registeredUser.Id },
                    registeredUser
                );
            }
            catch (Exception ex)
            {
                // Log the exception (you should have a logging mechanism)
                return StatusCode(500, "An error occurred during user registration");
            }
        }
    }
}
