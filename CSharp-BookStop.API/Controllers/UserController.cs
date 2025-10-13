using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CSharp_BookStop.API.Services;
using CSharp_BookStop.Database.Data;
using CSharp_BookStop.Database.Entities;
using Microsoft.AspNetCore.Authorization;

namespace CSharp_BookStop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(BookStopContext context, IUserService userService, 
        IAuthService authService, ITokenService tokenService) : ControllerBase
    {
        // GET: api/User/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<GetUserRequest>> GetUser(Guid id)
        {
            var user = await context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return new GetUserRequest
                (
                user.UserId, user.Email
            );
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> PutUser(Guid id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            context.Entry(user).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
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
        public async Task<ActionResult<GetUserRequest>> RegisterUser(RegisterUserRequest payload)
        {
            var result = await userService.RegisterUser(payload);

            return result.Status switch
            {
                HttpStatusCode.Conflict => Conflict(result.Message),
                HttpStatusCode.BadRequest => BadRequest(result.Message),
                _ => CreatedAtAction("GetUser", new { id = result.UserId },
                    new { id = result.UserId, email = result.UserEmail })
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> LoginUser(LoginUserRequest payload)
        {
            var result = await authService.LoginUser(payload);

            if (result.Status != HttpStatusCode.OK)
            {
                return BadRequest(result.Message);
            }


            
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTimeOffset.UtcNow.AddDays(14),
                SameSite = SameSiteMode.Lax,
            };
            HttpContext.Response.Cookies.Append("refreshToken", result.RefreshToken!, cookieOptions);
            return Ok(new TokenResponse(result.Jwt!));

        }

        // DELETE: api/User/5
        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(Guid id)
        {
            return context.Users.Any(e => e.UserId == id);
        }

        [Authorize]
        [HttpGet("/auth-test")]
        public string AuthTest()
        {
            return "You are authenticated!";
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/admin")]
        public string Admin()
        {
            return "You are admin!";
        }

        [HttpPost("refresh-tokens")]
        public async Task<ActionResult<TokenResponse>> RefreshTokens(RefreshTokenRequest payload)
        {
            var refreshTokenCookie = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshTokenCookie))
            {
                return BadRequest("Refresh token not found");
            }
            
            var token = await tokenService.RefreshTokens(payload.UserId, refreshTokenCookie);
            if (token == null)
            {
                return Unauthorized("Invalid refresh token.");
            }
            return Ok(new TokenResponse(token.Jwt));
        }
    }
}
