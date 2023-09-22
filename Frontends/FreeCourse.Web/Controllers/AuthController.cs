using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace FreeCourse.Web.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly IIdentityService _identityService;

    public AuthController(ILogger<AuthController> logger, IIdentityService identityService)
    {
        _logger = logger;
        _identityService = identityService;
    }

    public IActionResult SignIn()
    {
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await _identityService.RevokeRefreshToken();
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(SigninInput request)
    {
        if (!ModelState.IsValid) return View();

        var response = await _identityService.SignIn(request);

        if (!response.IsSuccessful)
        {
            response.Errors.ForEach(x => {
                ModelState.AddModelError(String.Empty, x);
            });
        
            return View();
        }

        return RedirectToAction(nameof(Index), "Home");
    }

}