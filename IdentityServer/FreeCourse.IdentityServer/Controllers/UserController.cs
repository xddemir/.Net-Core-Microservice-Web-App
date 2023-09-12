using System;
using System.Linq;
using System.Threading.Tasks;
using FreeCourse.IdentityServer.DTOs;
using FreeCourse.IdentityServer.Models;
using FreeCourse.Shared.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpDto request)
        {
            var user = new ApplicationUser()
            {
                UserName = request.UserName,
                Email = request.Email,
                City = request.City
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return BadRequest(Response<NoContent>.Fail(result.Errors.Select(x => x.Description).ToList(), 400));

            return NoContent();

        }
    }
}