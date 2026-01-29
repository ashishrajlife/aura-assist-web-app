using aura_assist_prod.DTOs;
using aura_assist_prod.Services;
using aura_assist_prod.Services.AuthTokenService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using aura_assist_prod.DTOs;
using aura_assist_prod.Services;
using System.Security.Claims;

namespace aura_assist_prod.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            ITokenBlacklistService tokenBlacklistService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _tokenBlacklistService = tokenBlacklistService;
            _logger = logger;
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

                // Log successful login
                _logger.LogInformation($"User {result.Email} logged in successfully");

                return Ok(new { success = true, data = result, message = "Login successful" });
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed login attempt for email: {loginDto.Email}");
                return Unauthorized(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Get current user info
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                // Get the token from request header
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                if (!string.IsNullOrEmpty(token))
                {
                    // Blacklist the token
                    await _tokenBlacklistService.BlacklistTokenAsync(token);

                    _logger.LogInformation($"User {userEmail} (ID: {userId}) logged out successfully");

                    return Ok(new
                    {
                        success = true,
                        message = "Logged out successfully. Token has been invalidated.",
                        timestamp = DateTime.UtcNow
                    });
                }

                return BadRequest(new { success = false, message = "No token provided" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { success = false, message = "Logout failed" });
            }
        }

        //[HttpPost("logout-all-devices")]
        //[Authorize]
        //public async Task<IActionResult> LogoutAllDevices()
        //{
        //    try
        //    {
        //        // Note: With in-memory cache, we cannot logout from all devices
        //        // without tracking all issued tokens per user
        //        // This is a limitation of Option A

        //        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //        var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        //        _logger.LogInformation($"User {userEmail} requested logout from all devices");

        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Please note: With current setup, logout only invalidates current token. For full logout from all devices, please change your password.",
        //            timestamp = DateTime.UtcNow
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error during logout all devices");
        //        return StatusCode(500, new { success = false, message = "Operation failed" });
        //    }
        //}

        [HttpGet("check-auth")]
        [Authorize]
        public IActionResult CheckAuth()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var userName = User.FindFirst("FullName")?.Value;
            var city = User.FindFirst("City")?.Value;

            return Ok(new
            {
                success = true,
                data = new
                {
                    userId,
                    userEmail,
                    userRole,
                    userName,
                    city,
                    isAuthenticated = true,
                    timestamp = DateTime.UtcNow
                }
            });
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                message = "API is working!",
                timestamp = DateTime.UtcNow,
                version = "1.0"
            });
        }
    }
}