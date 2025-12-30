using ImageApp.Application.Interfaces.Services;
using ImageApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ImageApp.Application
{
    public static class Register
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IRatingService, RatingService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ICreatorService, CreatorService>();

            return services;
        }
    }
}