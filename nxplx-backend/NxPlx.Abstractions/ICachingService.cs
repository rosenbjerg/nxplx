using System.Threading.Tasks;

namespace NxPlx.Abstractions
{
    public interface ICachingService
    {
        Task<string?> GetAsync(string key);
        
        Task SetAsync(string key, string value, CacheKind kind);
        
        Task RemoveAsync(string keyOrPattern);
    }
}