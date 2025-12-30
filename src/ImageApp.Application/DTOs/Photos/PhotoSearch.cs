namespace ImageApp.Application.DTOs.Photos
{
    public class PhotoSearch : PaginationRequest
    {
        public string Search { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}