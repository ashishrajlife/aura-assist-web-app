using aura_assist_prod.Data;
using aura_assist_prod.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aura_assist_prod.Data;
using aura_assist_prod.Helpers;
using aura_assist_prod.Models;
using System.Security.Claims;
using aura_assist_prod.DTOs;

namespace aura_assist_prod.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/dashboard/user-profile
        [HttpGet("user-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var user = await _context.Users
                    .Where(u => u.UserId == userId)
                    .Select(u => new
                    {
                        u.UserId,
                        u.FullName,
                        u.Email,
                        u.Phone,
                        u.Role,
                        u.IsApproved,
                        u.City,
                        u.CreatedAt
                    })
                    .FirstOrDefaultAsync();

                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/dashboard/influencers
        [HttpGet("influencers")]
        [Authorize(Roles = "User")] // Only regular users can view influencers
        public async Task<IActionResult> GetInfluencers([FromQuery] string city = null)
        {
            try
            {
                var query = _context.InfluencerProfiles
                    .Include(ip => ip.User)
                    .Where(ip => ip.User.IsApproved && ip.User.Role == "Influencer");

                // Filter by city if provided
                if (!string.IsNullOrEmpty(city))
                {
                    query = query.Where(ip => ip.User.City.Contains(city));
                }

                var influencers = await query
                    .Select(ip => new
                    {
                        ip.InfluencerProfileId,
                        ip.UserId,
                        UserName = ip.User.FullName,
                        ip.User.Email,
                        ip.User.Phone,
                        ip.User.City,
                        ip.InstagramHandle,
                        ip.TwitterHandle,
                        ip.WhatsappHandle,
                        ip.FollowersCount,
                        ip.Category,
                        ip.BasePrice,
                        IsContactable = !string.IsNullOrEmpty(ip.WhatsappHandle) ||
                                       !string.IsNullOrEmpty(ip.InstagramHandle) ||
                                       !string.IsNullOrEmpty(ip.TwitterHandle)
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = influencers });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/dashboard/agencies
        [HttpGet("agencies")]
        [Authorize(Roles = "User")] // Only regular users can view agencies
        public async Task<IActionResult> GetAgencies([FromQuery] string city = null)
        {
            try
            {
                var query = _context.AgencyProfiles
                    .Include(ap => ap.User)
                    .Where(ap => ap.User.IsApproved && ap.User.Role == "Agency");

                // Filter by city if provided
                if (!string.IsNullOrEmpty(city))
                {
                    query = query.Where(ap => ap.User.City.Contains(city) || ap.City.Contains(city));
                }

                var agencies = await query
                    .Select(ap => new
                    {
                        ap.AgencyProfileId,
                        ap.UserId,
                        AgencyName = ap.AgencyName,
                        UserName = ap.User.FullName,
                        ap.User.Email,
                        ap.User.Phone,
                        Location = ap.City ?? ap.User.City,
                        ap.InstagramHandle,
                        ap.TwitterHandle,
                        ap.WhatsappHandle,
                        ap.Services,
                        IsContactable = !string.IsNullOrEmpty(ap.WhatsappHandle) ||
                                       !string.IsNullOrEmpty(ap.InstagramHandle) ||
                                       !string.IsNullOrEmpty(ap.TwitterHandle)
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = agencies });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/dashboard/influencer-profile
        [HttpGet("influencer-profile")]
        [Authorize(Roles = "Influencer")] // Only influencers can access
        public async Task<IActionResult> GetInfluencerProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var profile = await _context.InfluencerProfiles
                    .Include(ip => ip.User)
                    .Where(ip => ip.UserId == userId)
                    .Select(ip => new
                    {
                        ip.InfluencerProfileId,
                        ip.UserId,
                        ip.User.FullName,
                        ip.User.Email,
                        ip.User.Phone,
                        ip.User.City,
                        ip.User.IsApproved,
                        ip.InstagramHandle,
                        ip.TwitterHandle,
                        ip.WhatsappHandle,
                        ip.FollowersCount,
                        ip.Category,
                        ip.BasePrice
                    })
                    .FirstOrDefaultAsync();

                if (profile == null)
                {
                    return NotFound(new { success = false, message = "Profile not found" });
                }

                return Ok(new { success = true, data = profile });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT: api/dashboard/update-influencer-profile
        [HttpPut("update-influencer-profile")]
        [Authorize(Roles = "Influencer")]
        public async Task<IActionResult> UpdateInfluencerProfile([FromBody] InfluencerProfileUpdateDto updateDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var profile = await _context.InfluencerProfiles
                    .FirstOrDefaultAsync(ip => ip.UserId == userId);

                if (profile == null)
                {
                    return NotFound(new { success = false, message = "Profile not found" });
                }

                // Update fields
                profile.InstagramHandle = updateDto.InstagramHandle;
                profile.TwitterHandle = updateDto.TwitterHandle;
                profile.WhatsappHandle = updateDto.WhatsappHandle;
                profile.FollowersCount = updateDto.FollowersCount;
                profile.Category = updateDto.Category;
                profile.BasePrice = updateDto.BasePrice;

                _context.InfluencerProfiles.Update(profile);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // GET: api/dashboard/agency-profile
        [HttpGet("agency-profile")]
        [Authorize(Roles = "Agency")] // Only agencies can access
        public async Task<IActionResult> GetAgencyProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var profile = await _context.AgencyProfiles
                    .Include(ap => ap.User)
                    .Where(ap => ap.UserId == userId)
                    .Select(ap => new
                    {
                        ap.AgencyProfileId,
                        ap.UserId,
                        AgencyName = ap.AgencyName,
                        ap.User.FullName,
                        ap.User.Email,
                        ap.User.Phone,
                        ap.User.City,
                        ap.User.IsApproved,
                        ap.InstagramHandle,
                        ap.TwitterHandle,
                        ap.WhatsappHandle,
                        ap.Services
                    })
                    .FirstOrDefaultAsync();

                if (profile == null)
                {
                    return NotFound(new { success = false, message = "Profile not found" });
                }

                return Ok(new { success = true, data = profile });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT: api/dashboard/update-agency-profile
        [HttpPut("update-agency-profile")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> UpdateAgencyProfile([FromBody] AgencyProfileUpdateDto updateDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var profile = await _context.AgencyProfiles
                    .FirstOrDefaultAsync(ap => ap.UserId == userId);

                if (profile == null)
                {
                    return NotFound(new { success = false, message = "Profile not found" });
                }

                // Update fields
                profile.AgencyName = updateDto.AgencyName;
                profile.InstagramHandle = updateDto.InstagramHandle;
                profile.TwitterHandle = updateDto.TwitterHandle;
                profile.WhatsappHandle = updateDto.WhatsappHandle;
                profile.Services = updateDto.Services;
                profile.City = updateDto.City;

                _context.AgencyProfiles.Update(profile);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}