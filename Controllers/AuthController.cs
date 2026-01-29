using aura_assist_prod.DTOs;
using Microsoft.AspNetCore.Mvc;
using aura_assist_prod.DTOs;
using aura_assist_prod.Services;
using System.Threading.Tasks;

namespace aura_assist_prod.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDto)
        {
            try
            {
                var result = await _authService.Register(registerDto);
                return Ok(new { success = true, data = result, message = "Registration successful" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            try
            {
                var result = await _authService.Login(loginDto);
                return Ok(new { success = true, data = result, message = "Login successful" });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "API is working!" });
        }
    }
}