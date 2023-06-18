using JWTDemo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private IConfiguration _config;
        private DemoJWTContext db;
        public LoginController(IConfiguration config)
        {
            _config = config;
        }
       
        private UserDTO Authen(UserDTO login)
        {
            List<UserDTO> user = new List<UserDTO>();
            user.Add(new UserDTO { UserName = "userdemo", Password = "1", Role = "user" });
            user.Add(new UserDTO { UserName = "admindemo", Password = "1", Role = "admin" });

           UserDTO userLogin = user.Where(x => x.UserName.Equals(login.UserName) && x.Password.Equals(login.Password)).FirstOrDefault();
            
            return userLogin;
        }

        [HttpGet]
        public IActionResult Login (string username, string password)
        { 
            IActionResult respone = Unauthorized();
            try
            {
               UserDTO userDTO= new UserDTO();
                userDTO.UserName = username;
                userDTO.Password = password;

                var user = Authen(userDTO);
                if (user != null)
                {
                    var tokenStr = GenerateJSONToken(user);
                    respone = Ok(new { token = tokenStr });
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
            return respone;
        }

        private string GenerateJSONToken(UserDTO userToken)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Name,userToken.UserName),
                new Claim(JwtRegisteredClaimNames.Sub,userToken.Role),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
               audience: _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials
                );
            var encodetoken = new JwtSecurityTokenHandler().WriteToken(token);
            return encodetoken;
        }


        [Authorize]
        [HttpPost("Post")]
        public string Post()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            IList<Claim> claims = identity.Claims.ToList();
            var roler = claims[1].Value;
            if (roler.Equals("user"))
            {
                return " Xin Chào User";
            }else if (roler.Equals("admin")){
                return "Xin Chào Admin";
            }
            else
            {
                return "Lỗi";
            }
           
            
        }

        [Authorize]
        [HttpGet("GetValue")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "a1", "a2", "a3" };
        }
    }
}
