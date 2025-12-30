using System.ComponentModel.DataAnnotations;

namespace ImageApp.Web.Models;

public sealed class CreatorUploadVm
{
    [Required, MinLength(2), MaxLength(200)]
    public string Title { get; set; } = "";

    [MaxLength(2000)]
    public string? Caption { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    // comma separated
    [MaxLength(2000)]
    public string? PeoplePresent { get; set; }

    // comma separated
    [MaxLength(2000)]
    public string? Tags { get; set; }

    [Required]
    public IFormFile? File { get; set; }
}