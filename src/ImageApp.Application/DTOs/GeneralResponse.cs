namespace ImageApp.Application.DTOs
{
    public class GeneralResponse<T>(string message = "Success", T? data = default, int code = 00)
    {
        public T? Data { get; init; } = data;
        public string Message { get; init; } = message;
        public int Code { get; init; } = code;
        public bool Success { get; set; } = code == 00;
    }
}