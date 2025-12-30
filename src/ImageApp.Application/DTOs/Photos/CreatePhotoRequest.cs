namespace ImageApp.Application.DTOs.Photos
{
    public class CreatePhotoRequest
    {
        public required string Title { get; set; }
        public string? Caption { get; set; }
        public string? Location { get; set; }
        public string[]? PeoplePresent { get; set; }
        public string[]? Tags { get; set; }
        public required string Base64Image { get; set; }
    }
}