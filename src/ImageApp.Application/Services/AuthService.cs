using ImageApp.Application.DTOs.Auth;
using ImageApp.Application.Interfaces;
using ImageApp.Domain.Entities;
using ImageApp.Domain.Enums;
using ImageApp.Application.DTOs;
using ImageApp.Application.DTOs.Constants;
using ImageApp.Application.Interfaces.Repositories;
using ImageApp.Application.Interfaces.Services;

namespace ImageApp.Application.Services;

public class AuthService(IUserRepository users, IJwtTokenGenerator jwt, IRedisCacheService redisCacheService) : IAuthService
{
    public async Task<GeneralResponse<AuthResponse>> RegisterAsync(RegisterRequest req, CancellationToken cancellationToken)
    {
        var email = req.Email.Trim().ToLowerInvariant();

        if (await users.EmailExistsAsync(email, cancellationToken))
            return new GeneralResponse<AuthResponse>("Email already exists.", null, 01);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            DisplayName = req.DisplayName.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = UserRole.Consumer
        };

        await users.AddAsync(user, cancellationToken);
        await users.SaveChangesAsync(cancellationToken);

        var userCacheKey = $"user-{email}";
        await redisCacheService.SetDataAsync<User>(userCacheKey, user, CacheTime.TwentyFourHoursInSeconds, cancellationToken);

        var token = jwt.Generate(user);
        return new GeneralResponse<AuthResponse>("Success", new AuthResponse(user.Id, user.Email, user.DisplayName, user.Role.ToString(), token));
    }

    public async Task<GeneralResponse<AuthResponse>> LoginAsync(LoginRequest req, CancellationToken cancellationToken)
    {
        var email = req.Email.Trim().ToLowerInvariant();

        var userCacheKey = $"user-{req.Email}";

        var user = await redisCacheService.GetDataAsync<User>(userCacheKey, cancellationToken)
                   ?? await users.GetByEmailAsync(email, cancellationToken);

        if (user is null)
            return new GeneralResponse<AuthResponse>(message: "Invalid credentials.", null, 01);

        await redisCacheService.SetDataAsync<User>(userCacheKey, user, CacheTime.TwentyFourHoursInSeconds, cancellationToken);

        if (!BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return new GeneralResponse<AuthResponse>("Invalid credentials.", null, 01);

        var token = jwt.Generate(user);
        return new GeneralResponse<AuthResponse>("Success",
            new AuthResponse(user.Id, user.Email, user.DisplayName, user.Role.ToString(), token));
    }

    public async Task<GeneralResponse<AuthResponse>> GetMyInfoAsync(CancellationToken cancellationToken)
    {
        var email = jwt.CurrentUsername();

        var userCacheKey = $"user-{email}";

        var user = await redisCacheService.GetDataAsync<User>(userCacheKey, cancellationToken)
                   ?? await users.GetByEmailAsync(email ?? "", cancellationToken);

        if (user is null)
            return new GeneralResponse<AuthResponse>(message: "User not found.", null, 01);

        await redisCacheService.SetDataAsync<User>(userCacheKey, user, CacheTime.TwentyFourHoursInSeconds, cancellationToken);

        var token = jwt.Generate(user);
        return new GeneralResponse<AuthResponse>("Success", new AuthResponse(user.Id, user.Email, user.DisplayName, user.Role.ToString(), token));
    }

    public async Task UpdateUserCacheAsynce(Guid userId, CancellationToken cancellationToken)
    {
        var user = await users.GetByIdAsync(userId, cancellationToken);

        var userCacheKey = $"user-{user?.Email}";

        if (user is not null)
            await redisCacheService.SetDataAsync<User>(userCacheKey, user, CacheTime.TwentyFourHoursInSeconds,
                cancellationToken);
    }
}