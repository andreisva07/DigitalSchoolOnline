using AppAPI.Context;
using AppAPI.Helpers;
using AppAPI.Models;
using AppAPI.Models.Dto;
using AppAPI.UtilityService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;

        public UserController(AppDbContext appDbContext, IConfiguration configuration, IEmailService emailService, IJwtService jwtService)
        {
            _context = appDbContext;
            _configuration = configuration;
            _emailService = emailService;
            _jwtService = jwtService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User user)
        {
            if (user == null)
                return BadRequest();

            var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == user.Username);

            if (existingUser == null)
                return NotFound(new { Message = "User not found" });

            if (!PasswordHasher.VerifyPassword(user.Password, existingUser.Password))
            {
                return BadRequest(new { Message = "Password is incorrect" });
            }

            string role = existingUser.Role;
            if (role == "Admin" || role == "Teacher" || role == "Student")
            {
                existingUser.Token = _jwtService.CreateJwt(existingUser, existingUser.CNP);

                var newAccessToken = existingUser.Token;
                var newRefreshToken = CreateRefreshToken();
                existingUser.RefreshToken = newRefreshToken;
                existingUser.RefreshTokenExpiryTime = DateTime.Now.AddDays(5);
                await _context.SaveChangesAsync();

                return Ok(new TokenApiDto()
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            else
            {
                return BadRequest(new { Message = "Unknown role" });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest();

            if (await CheckUsernameExist(user.Username))
                return BadRequest(new { Message = "Username Already Exist" });

            if (await CheckEmailExist(user.Email))
                return BadRequest(new { Message = "Email Already Exist" });

            user.Password = PasswordHasher.HashPassword(user.Password);
            user.Role = "Admin";
            user.Token = _jwtService.CreateJwt(user, user.CNP);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                Message = "User registered",
                Token = user.Token
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            return Ok(await _context.Users.ToListAsync());
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(TokenApiDto tokenApiDto)
        {
            if (tokenApiDto is null)
                return BadRequest("Invalid Client Request");
            string accessToken = tokenApiDto.AccessToken;
            string refreshToken = tokenApiDto.RefreshToken;
            var principal = GetPrincipleFromExpiredToken(accessToken);
            var username = principal.Identity.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("Invalid Request");
            var newAccessToken = _jwtService.CreateJwt(user, user.CNP); // Adăugare CNP în token
            var newRefreshToken = CreateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _context.SaveChangesAsync();
            return Ok(new TokenApiDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            });
        }

        private async Task<bool> CheckUsernameExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username);
        }

        private async Task<bool> CheckEmailExist(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        private string CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var tokenInUser = _context.Users
                .Any(a => a.RefreshToken == refreshToken);
            if (tokenInUser)
            {
                return CreateRefreshToken();
            }
            return refreshToken;
        }

        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes("secretkeyeryrqn46hxjny");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid Token");
                return principal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost("send-reset-email/{email}")]
        public async Task<IActionResult> SendEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 400,
                    Message = "email doesn't exist"
                });
            }
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var emailToken = Convert.ToBase64String(tokenBytes);
            user.ResetPasswordTOken = emailToken;
            user.ResetPasswordExpiry = DateTime.Now.AddMinutes(15);
            string from = _configuration["EmailSettings:Form"];
            var emailModel = new EmailModel(email, "Reset password", EmailBody.EmailStringBody(email, emailToken));
            _emailService.SendEmail(emailModel);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                StatusCode = 200,
                Message = "Email Sent"
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var newToken = resetPasswordDto.EmailToken.Replace("", "+");
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(a => a.Email == resetPasswordDto.Email);
            if (user == null)
            {
                return NotFound(new
                {
                    StatusCode = 400,
                    Message = "user doesn't exist"
                });
            }
            var tokenCode = user.ResetPasswordTOken;
            DateTime emailTokenExpiry = user.ResetPasswordExpiry;
            if (tokenCode != resetPasswordDto.EmailToken || emailTokenExpiry < DateTime.Now)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid Reset link"
                });
            }
            user.Password = PasswordHasher.HashPassword(resetPasswordDto.NewPassword);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                StatusCode = 200,
                Message = "password reset succesfully"
            });
        }
    }
}
