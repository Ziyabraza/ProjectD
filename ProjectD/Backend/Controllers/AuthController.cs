using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.IO;
using System.Linq;

namespace ProjectD
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Backend", "Data", "users.json");
            var userFile = System.IO.File.ReadAllText(filePath);
            var users = JsonConvert.DeserializeObject<List<User>>(userFile);
            

            var user = users.FirstOrDefault(u =>
                u.Username == request.Username && u.Password == request.Password);

            if (user == null)
                new Error(302, Request.Path, "Invalid username or password.");

            
            var key = Encoding.UTF8.GetBytes("!!H0g3ScH00LR0tt3RdAm@2025-04-22??");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, request.Username),
                    new Claim(ClaimTypes.Role, user.Role) 
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                username = user.Username,
                role = user.Role
            });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Backend", "Data", "users.json");

            var userFile = System.IO.File.ReadAllText(filePath);
            var users = JsonConvert.DeserializeObject<List<User>>(userFile) ?? new List<User>();

            if (users.Any(u => u.Username == request.Username))
            {
                new Error(302, Request.Path, "Username already exists.");
            }

            var newUser = new User
            {
                Username = request.Username,
                Password = request.Password,
                Role = "User"
            };

            users.Add(newUser);

            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, json);

            return Ok("User registered successfully.\n" +
                $"Username = {newUser.Username}\n" + 
                $"Role = {newUser.Role}");
        }
    }

    public class LoginRequest
    {   
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}