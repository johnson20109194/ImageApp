using ImageApp.Domain.Entities;

namespace ImageApp.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string Generate(User user);

        string? CurrentUsername();

        Guid CurrentUserId();
    }
}