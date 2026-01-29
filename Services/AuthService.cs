using aura_assist_prod.Data;
using aura_assist_prod.DTOs;
using aura_assist_prod.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace aura_assist_prod.Services
{
    public interface IAuthService
    {
        Task<UserResponseDTO> Register(RegisterDTO registerDto);
        Task<UserResponseDTO> Login(LoginDTO loginDto);
        string GenerateJwtToken(User user);
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Add this method in AuthService class
        public async Task<UserResponseDTO> Register(RegisterDTO registerDto)
        {
            // Validate role
            var validRoles = new[] { "User", "Influencer", "Agency" };
            if (!validRoles.Contains(registerDto.Role))
            {
                throw new Exception($"Invalid role. Allowed roles: {string.Join(", ", validRoles)}");
            }

            // Check if user already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

            if (existingUser != null)
            {
                throw new Exception("User with this email already exists");
            }

            // Create new user
            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Phone = registerDto.Phone,
                Role = registerDto.Role,
                City = registerDto.City,
                IsApproved = registerDto.Role == "User" // Auto-approve regular users
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // If user registered as Influencer, create influencer profile
            if (registerDto.Role == "Influencer")
            {
                var influencerProfile = new InfluencerProfile
                {
                    UserId = user.UserId
                };
                _context.InfluencerProfiles.Add(influencerProfile);
                await _context.SaveChangesAsync();
            }

            // If user registered as Agency, create agency profile
            if (registerDto.Role == "Agency")
            {
                var agencyProfile = new AgencyProfile
                {
                    UserId = user.UserId
                };
                _context.AgencyProfiles.Add(agencyProfile);
                await _context.SaveChangesAsync();
            }

            // Generate token
            var token = GenerateJwtToken(user);

            return new UserResponseDTO
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                IsApproved = user.IsApproved,
                City = user.City,
                CreatedAt = user.CreatedAt,
                Token = token
            };
        }

        public async Task<UserResponseDTO> Login(LoginDTO loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password");
            }

            // Generate token
            var token = GenerateJwtToken(user);

            return new UserResponseDTO
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                IsApproved = user.IsApproved,
                City = user.City,
                CreatedAt = user.CreatedAt,
                Token = token
            };
        }

        public string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FullName", user.FullName),
                new Claim("City", user.City ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}