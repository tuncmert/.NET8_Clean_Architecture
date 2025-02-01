using App.API.ExceptionHandler;
using App.Application.Contracts.Caching;
using App.Caching;

namespace App.API.Extensions
{
    public static class CachingExtensions
    {
        public static IServiceCollection AddCachingExt(this IServiceCollection services)
        {
            services.AddSingleton<ICacheService, CacheService>();

            services.AddMemoryCache();
            return services;
        }
    }
}
