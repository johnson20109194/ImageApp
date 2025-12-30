namespace ImageApp.Application.Interfaces.Services;

public interface IRedisCacheService
{
    Task<T?> GetDataAsync<T>(string key, CancellationToken cancellationToken = default);
    Task<bool> RemoveDataAsync(string key, CancellationToken cancellationToken = default);
    Task<bool> SetDataAsync<T>(string key, T value, int ttl = 300, CancellationToken cancellationToken = default);
}