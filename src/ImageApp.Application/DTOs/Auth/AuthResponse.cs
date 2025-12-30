namespace ImageApp.Application.DTOs.Auth;

public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string Role { get; set; } = default!;
    public string Token { get; set; } = default!;

    public AuthResponse(Guid userId, string email, string displayName, string role, string token)
    {
        UserId = userId;
        Email = email;
        DisplayName = displayName;
        Role = role;
        Token = token;
    }
}