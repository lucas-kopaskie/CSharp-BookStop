using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSharp_BookStop.API.Helpers;
using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Models;
using Microsoft.IdentityModel.Tokens;

namespace CSharp_BookStop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly BookStopContext _context;
        private readonly IConfiguration _configuration;
        public UserController(BookStopContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/User/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GetUserDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return new GetUserDto
                (
                user.UserId, user.Email
            );
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        public async Task<ActionResult<GetUserDto>> RegisterUser(RegisterUserDto payload)
        {

            if (payload.Password.Equals(payload.ConfrimPassword).Equals(false))
            {
                return BadRequest("Passwords do not match");
            }

            if (await _context.Users.AnyAsync(u => u.Email == payload.Email))
            {
                return Conflict("Email already exists");
            }
            
            var salt = RandomNumberGenerator.GetBytes(64);
            
            var hashedPassword = UserHelpers.HashPassword(payload.Password, salt);

            User user = new()
            {
                UserId = Guid.NewGuid(),
                Email = payload.Email,
                PasswordSalt = Convert.ToHexString(salt),
                PasswordHash = hashedPassword
            };

            UserCart userCart = new()
            {
                UserCartId = Guid.NewGuid(),
                UserId = user.UserId
            };
            
            _context.Users.Add(user);
            _context.UserCarts.Add(userCart);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, new { id = user.UserId, email = user.Email });
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUser(LoginUserDto payload)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

            if (user == null)
            {
                return NotFound($"No user found with the email: {payload.Email}");
            }
            
            var passwordMatch = UserHelpers.VerifyPassword(payload.Password, Convert.FromHexString(user.PasswordSalt), user.PasswordHash);
            if (!passwordMatch)
            {
                return Unauthorized("Invalid username/password.");
            }
            
            IEnumerable<Claim> claims =
            [
                new (JwtRegisteredClaimNames.Sub, user.UserId.ToString(), ClaimValueTypes.String),
                new (JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new (JwtRegisteredClaimNames.AuthTime, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new (JwtRegisteredClaimNames.Nonce, Guid.NewGuid().ToString(), ClaimValueTypes.String),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString(), ClaimValueTypes.String),
                new (JwtRegisteredClaimNames.Email, user.Email, ClaimValueTypes.Email),
            ];
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration["jwtSigningCredentials"]!;


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = "localhost",
                Audience = "localhost",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new GetAuthenticationTokenDto(user.UserId, "JWT", jwt));
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
