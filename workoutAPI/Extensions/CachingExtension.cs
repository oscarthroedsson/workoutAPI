using EasyCaching.InMemory;

namespace workoutAPI.Extensions;
using EasyCaching.Core.Configurations;
using Microsoft.Extensions.DependencyInjection;



public static class CachingExtensions
{
    public static IServiceCollection AddCustomCaching(this IServiceCollection services)
    {
        services.AddEasyCaching(options =>
        {
            options.UseInMemory(opt =>
            {
                opt.DBConfig = new InMemoryCachingOptions()
                {
                    ExpirationScanFrequency = 60,
                    SizeLimit = 10000,
                    EnableReadDeepClone = true,
                    EnableWriteDeepClone = false
                };
                opt.MaxRdSecond = 0;
                opt.EnableLogging = true;
            }, "default");

            // BillingRecord provider
            options.UseInMemory(opt =>
            {
                opt.DBConfig = new InMemoryCachingOptions
                {
                    ExpirationScanFrequency = 120,
                    SizeLimit = 2000,
                    EnableReadDeepClone = true,
                    EnableWriteDeepClone = false
                };
                opt.MaxRdSecond = 0;
                opt.EnableLogging = true;
            }, "BillingRecord");

            // Här kan du enkelt lägga till fler providers i framtiden
        });

        return services;
    }
}
