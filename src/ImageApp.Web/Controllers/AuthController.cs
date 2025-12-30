using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ImageApp.Application.DTOs.Auth;
using ImageApp.Application.Interfaces.Services;

namespace ImageApp.Web.Controllers;

public class AuthController(IAuthService auth) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string email, string displayName, string password, string? returnUrl = null, CancellationToken ct = default)
    {
        var result = await auth.RegisterAsync(new RegisterRequest
        {
            Email = email,
            DisplayName = displayName,
            Password = password
        }, ct);
        if (!result.Success)
        {
            ModelState.AddModelError("", result.Message!);
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        if (result.Data is null) return RedirectToAction("Index", "Home");

        await SignInAsync(result.Data);
        return LocalRedirect(returnUrl ?? "/");
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest req, CancellationToken ct)
    {
        var res = await auth.LoginAsync(req, ct);

        if (res.Data is null) return RedirectToAction("Index", "Home");

        await SignInAsync(res.Data);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied() => View();

    private async Task SignInAsync(AuthResponse result)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.UserId.ToString()),
            new(ClaimTypes.Name, result.DisplayName ?? result.Email ?? ""),
            new(ClaimTypes.Email, result.Email ?? ""),
            new(ClaimTypes.Role, result.Role ?? "Viewer")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true });
    }
}