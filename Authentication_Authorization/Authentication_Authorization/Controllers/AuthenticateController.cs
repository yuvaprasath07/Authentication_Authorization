using Authentication_Authorization.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication_Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JWTSettings _jwtSettings;

        public AuthenticateController(ApplicationDbContext context, IOptions<JWTSettings> options)
        {
            _context = context;
            _jwtSettings = options.Value;
        }

        [HttpPost("GenerateToken")]

        public IActionResult Authenticate([FromBody] UserCreate user)
        {
            var _user = _context.UserLogin.FirstOrDefault(a => a.UserName == user.UserName && a.Password == a.Password);
            if (_user == null)
                return Unauthorized();

            var token = GenerateToken(_user);
            return Ok(token);
        }

        private object GenerateToken(UserCreate user)
        {
            try
            {
               var tokenHandler=new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.securitykey);


                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject=new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserName",user.UserName),
                        new Claim(ClaimTypes.Role,user.Role),
                    }),
                    Expires=DateTime.UtcNow.AddMinutes(1),
                    SigningCredentials= new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to generate token.", ex);
            }
            
        }

        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken (string refreshToken)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.securitykey);

                var validatedToken = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out var securityToken);
                var username = validatedToken.Claims.First(x => x.Type == "Username").Value;
                var role = validatedToken.Claims.First(x => x.Type == ClaimTypes.Role).Value;

                var newRefreshTokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("Username", username),
                        new Claim(ClaimTypes.Role, role)
                    }),
                    Expires = System.DateTime.Now.AddMinutes(11),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var newRefreshToken = tokenHandler.CreateToken(newRefreshTokenDescriptor);
                var newRefreshTokenString = tokenHandler.WriteToken(newRefreshToken);
                return Ok(newRefreshTokenString);
            }
            catch(Exception ex)
            {
                throw new Exception("Failed to generate refresh token.", ex);
            }
        }
    }
}
