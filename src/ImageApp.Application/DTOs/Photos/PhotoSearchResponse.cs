namespace ImageApp.Application.DTOs.Photos;

public class PhotoSearchResponse(
    IReadOnlyList<PhotoResponse> items,
    int total,
    int page,
    int pageSize)
{
    public IReadOnlyList<PhotoResponse> Items { get; } = items;
    public int Total { get; } = total;
    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;

    public int TotalPages => (int)Math.Ceiling(Total / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}