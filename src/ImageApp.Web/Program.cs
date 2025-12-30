using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using ImageApp.Application;
using ImageApp.Application.Interfaces;
using ImageApp.Infrastructure;
using ImageApp.Infrastructure.Identity;
using ImageApp.Infrastructure.Persistence;
using ImageApp.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        // or ReferenceHandler.Preserve (adds $id/$ref in JSON)
    });

// Cookie auth is best suited for MVC pages
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/Auth/Login";
        opt.AccessDeniedPath = "/Auth/Denied";
        opt.SlidingExpiration = true;
        opt.ExpireTimeSpan = TimeSpan.FromHours(8);
        opt.Cookie.SameSite = SameSiteMode.Lax;

        // IMPORTANT for http://localhost in dev
        opt.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// Auth/JWT
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
//builder.Services.AddSingleton<JwtTokenGenerator>();

// DB (MySQL example)
// Connection string key should match your existing configuration.
var dbConn = builder.Configuration.GetConnectionString("DbConnection")
            ?? builder.Configuration["ConnectionStrings:DbConnection"]
            ?? throw new InvalidOperationException("ConnectionStrings:DbConnection missing");

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseMySql(dbConn, ServerVersion.AutoDetect(dbConn));
});

// Redis distributed cache (optional)
var redisConn = builder.Configuration.GetConnectionString("RedisConnection")
              ?? builder.Configuration["ConnectionStrings:RedisConnection"];

if (!string.IsNullOrWhiteSpace(redisConn))
{
    builder.Services.AddStackExchangeRedisCache(opt =>
    {
        opt.Configuration = redisConn; // e.g. local-cache:6379,abortConnect=false
    });
}

// Blob Storage (your existing AddAzureClients + Storage options)
// builder.Services.AddAzureClients(...)
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["Storage:ConnectionString"]);
});
builder.Services.AddScoped<IMediaStorage, AzureBlobMediaStorage>();

builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 10 * 1024 * 1024); //10 MB file upload limit

// App services
builder.Services.AddApplication();

// Register Infrastructure + Application via extension methods
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Lax
});

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();