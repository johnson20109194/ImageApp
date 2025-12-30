using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ImageApp.Application.Interfaces.Services;
using ImageApp.Web.Models;

namespace ImageApp.Web.Controllers;

public class HomeController(IPhotoService photos, ILogger<HomeController> logger) : Controller
{
    // public IActionResult Index()
    // {
    //     return View();
    // }

    public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 12, CancellationToken ct = default)
    {
        var model = await photos.SearchAsync(q, "", page, pageSize, ct);
        ViewBag.Query = q ?? "";
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}