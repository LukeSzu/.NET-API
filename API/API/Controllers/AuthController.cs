using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using API.Dtos;
using API.Models;
using System.Text.RegularExpressions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var user = new User { 
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                City = model.City,
                Address = model.Address
            };
            var errors = new List<IdentityError>();
            if (user.UserName.Length < 5)
            {
                errors.Add(new IdentityError { Code = "shortUsername", Description = "Username must have at least 5 letters." });
            }
            if (!Regex.IsMatch(user.PhoneNumber, @"^\d{9}$"))
            {
                errors.Add(new IdentityError { Code = "BadNumber", Description = "Phone number isn't in 123456789 format." });
            }
            if (!Regex.IsMatch(user.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                errors.Add(new IdentityError { Code = "BadEmail", Description = "Check your email adress." });
            }
            if(user.City == "")
            {
                errors.Add(new IdentityError { Code = "EmptyCity", Description = "City cannot be empty." });
            }
            if (user.Address == "")
            {
                errors.Add(new IdentityError { Code = "EmptyAddress", Description = "Address cannot be empty." });
            }
            if (errors.Count() > 0)
            {
                return BadRequest(errors);
            }
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                var token = GenerateJwtToken(user);

                return Ok(new { Token = token, Username = user.UserName });
            }

            return Unauthorized();
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
