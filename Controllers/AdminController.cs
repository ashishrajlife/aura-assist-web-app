using aura_assist_prod.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using aura_assist_prod.Data;

namespace aura_assist_prod.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/pending-approvals
        [HttpGet("pending-approvals")]
        public async Task<IActionResult> GetPendingApprovals()
        {
            try
            {
                var pendingUsers = await _context.Users
                    .Where(u => !u.IsApproved && (u.Role == "Influencer" || u.Role == "Agency"))
                    .Select(u => new
                    {
                        u.UserId,
                        u.FullName,
                        u.Email,
                        u.Phone,
                        u.Role,
                        u.City,
                        u.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new { success = true, data = pendingUsers });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT: api/admin/approve-user/{id}
        [HttpPut("approve-user/{id}")]
        public async Task<IActionResult> ApproveUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                user.IsApproved = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "User approved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        // PUT: api/admin/reject-user/{id}
        [HttpDelete("reject-user/{id}")]
        public async Task<IActionResult> RejectUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "User rejected and removed" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}