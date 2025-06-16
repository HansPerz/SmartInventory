using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartInventory.Data;
using SmartInventory.DTO;
using SmartInventory.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly InventoryDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(InventoryDbContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] DTO.LoginRequest request)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == request.Username);

            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }
            var password = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (password == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid credentials");
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return Ok(new   
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

    }
}
