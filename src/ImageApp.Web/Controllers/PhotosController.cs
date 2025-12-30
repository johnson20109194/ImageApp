using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ImageApp.Application.DTOs.Comments;
using ImageApp.Application.DTOs.Ratings;
using ImageApp.Application.Interfaces.Services;

namespace ImageApp.Web.Controllers;

public class PhotosController(
    IPhotoService photoService,
    ICommentService commentService,
    IRatingService ratingService) : Controller
{

    [HttpGet("/photos/{id:guid}")]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        var photo = await photoService.GetByIdAsync(id, ct);

        var comments = await commentService.GetByPhotoIdAsync(photo.Id, ct);
        photo.Comments = comments.ToList();

        return View(photo);
    }

    [Authorize]
    [HttpPost("/photos/{id:guid}/comment")]
    public async Task<IActionResult> Comment(Guid id, string text, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(text))
            return RedirectToAction(nameof(Details), new { id });

        await commentService.AddAsync(id, new CreateCommentRequest
        {
            Text = text.Trim()
        }, ct);
        return RedirectToAction(nameof(Details), new { id });
    }

    [Authorize]
    [HttpPost("/photos/{id:guid}/rate")]
    public async Task<IActionResult> Rate(Guid id, int score, CancellationToken ct)
    {
        score = Math.Clamp(score, 1, 5);
        await ratingService.RateAsync(id, new CreateRatingRequest { Score = score }, ct);
        return RedirectToAction(nameof(Details), new { id });
    }
}