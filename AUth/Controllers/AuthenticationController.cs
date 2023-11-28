namespace AUth.Controllers
{
    using AUth.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic; 
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if(user!=null && await _userManager.CheckPasswordAsync(user,model.password))
            {
                var userRole = await _userManager.GetRolesAsync(user);

                var AuthClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                };

                foreach(var role in userRole)
                {
                    AuthClaims.Add(new Claim(ClaimTypes.Role, role));
                }
                var authsigninkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: AuthClaims,
                    signingCredentials: new SigningCredentials(authsigninkey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    User = user.UserName,
                });
            }

            return Unauthorized();
        }


        [HttpPost("SignUp")]
        public async Task<IActionResult> Signup([FromBody] RegisterModel model)
        {                                              
            var userexist = await _userManager.FindByNameAsync(model.UserName);
            if (userexist != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Responce { status = "Exists", message = "user already exist" });
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };
            var result = await _userManager.CreateAsync(user, model.password);
            if(!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Responce { status = "Error", message = "user craetion failed" });
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.User))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Responce { status = "Success", message = "user craeted succesfully" });
        }

        [HttpPost("Admin")]
        public async Task<IActionResult> Admin([FromBody] RegisterModel model)
        {
            var userexist = await _userManager.FindByNameAsync(model.UserName);
            if (userexist != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Responce { status = "Error", message = "user already exist" });
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };
            var result = await _userManager.CreateAsync(user, model.password);
            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Responce { status = "Error", message = "user craetion failed" });
            }
            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            return Ok(new Responce { status = "Success", message = "Admin craeted succesfully" });
        }
    }
}
