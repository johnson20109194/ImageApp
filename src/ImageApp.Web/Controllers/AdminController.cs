using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ImageApp.Application.DTOs.Admin;
using ImageApp.Application.Interfaces.Services;
using ImageApp.Domain.Enums;

namespace ImageApp.Web.Controllers;

[Authorize(Roles = nameof(UserRole.Admin))]
public class AdminController(IAdminService admin) : Controller
{
    [HttpGet]
    public IActionResult Users() => View(Array.Empty<ImageApp.Application.DTOs.Admin.UserSearchResult>());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Users(string q, CancellationToken ct)
    {
        var results = await admin.SearchUsersAsync(q ?? "", ct);
        ViewBag.Query = q ?? "";
        return View(results);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetRole(Guid userId, string role, CancellationToken ct)
    {
        if (!Enum.TryParse<UserRole>(role, out var parsed))
            parsed = UserRole.Consumer;

        await admin.ChangeRole(new ChangeRoleRequest { UserId = userId, UserRole = parsed }, ct);
        return RedirectToAction(nameof(Users), new { });
    }
}