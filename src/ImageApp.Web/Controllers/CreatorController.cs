using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ImageApp.Application.DTOs.Photos;
using ImageApp.Application.Interfaces.Services;
using ImageApp.Domain.Enums;
using ImageApp.Web.Models;

namespace ImageApp.Web.Controllers;

[Authorize(Roles = $"{nameof(UserRole.Creator)},{nameof(UserRole.Admin)}")]
public class CreatorController(ICreatorService creator) : Controller
{
    [HttpGet]
    public IActionResult Upload() => View(new CreatorUploadVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(CreatorUploadVm vm, CancellationToken ct)
    {
        //if (!ModelState.IsValid) return View(vm);

        try
        {
            var req = new CreatePhotoRequest
            {
                Title = vm.Title,
                Location = vm.Location,
                Caption = vm.Caption,
                PeoplePresent = SplitCsv(vm.PeoplePresent).ToArray(),
                Tags = SplitCsv(vm.Tags).ToArray(),
                Base64Image = null
            };

            var id = await creator.UploadPhotoAsync(req, vm.File!, ct);
            return RedirectToAction("Details", "Photos", new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(vm);
        }
    }

    private static IReadOnlyList<string> SplitCsv(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return [];

        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(25)
            .ToList();
    }
}