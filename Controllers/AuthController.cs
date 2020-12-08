using AuthServices.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        static List<User> users;
        private IConfiguration _config;
        static AuthController()
        {
            users = new List<User>() {
            new User(){UserId=1,UserName="Alisha",Password="alisha"},
            new User(){UserId=2,UserName="Sai",Password="Patnana"}
            };
        }
        public AuthController(IConfiguration configuration)
        {
            _config = configuration;
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(users);
        }
        [HttpPost]
        public IActionResult PostUser([FromBody] User user)
        {
            users.Add(user);
            return Ok();
        }
        [HttpPost("GetUser")]
        public User GetUser(User valuser)
        {
            var user = users.FirstOrDefault(c => c.UserName == valuser.UserName && c.Password == valuser.Password);
            if (user == null)
            {
                return null;
            }
            return user;
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] User login)
        {
            IActionResult response = Unauthorized();
            User user = GetUser(login);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                var tokenString = GenerateJSONWebToken(login);
                response = Ok(new { token = tokenString });
                return response;
            }
        }
        private string GenerateJSONWebToken(User userInfo)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddSeconds(10),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
