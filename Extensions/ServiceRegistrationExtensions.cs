using System.Globalization;
using System.Net;
using JourneyFinder.Builders;
using JourneyFinder.Factories;
using JourneyFinder.Filters;
using JourneyFinder.Helpers;
using JourneyFinder.Managers;
using JourneyFinder.Managers.Interfaces;
using JourneyFinder.Options;
using JourneyFinder.Services;
using JourneyFinder.Services.Interfaces;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;

namespace JourneyFinder.Extensions;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IRedisCacheService, RedisCacheService>()
            .AddScoped<ISessionService, SessionService>()
            .AddScoped<IBusLocationService, BusLocationService>()
            .AddScoped<IJourneyService, JourneyService>();
    }

    public static IServiceCollection AddCustomManagers(this IServiceCollection services)
    {
        return services
            .AddScoped<ISessionManager, SessionManager>()
            .AddScoped<IJourneyManager, JourneyManager>()
            .AddScoped<IBusLocationManager, BusLocationManager>();
    }

    public static IServiceCollection AddCustomBuilders(this IServiceCollection services)
    {
        return services
            .AddScoped<IHomeViewModelBuilder, HomeViewModelBuilder>()
            .AddScoped<IJourneyViewModelBuilder, JourneyViewModelBuilder>();
    }

    public static IServiceCollection AddCustomFactories(this IServiceCollection services)
    {
        return services
            .AddScoped<IDeviceRequestFactory, DeviceRequestFactory>()
            .AddScoped<ICookieManager, CookieManager>();
    }

    public static IServiceCollection AddCustomFilters(this IServiceCollection services)
    {
        return services
            .AddScoped<JourneyValidationFilter>();
    }
    
    public static IServiceCollection AddCustomHelpers(this IServiceCollection services)
    {
        return services
            .AddScoped<IRequestContextHelper,  RequestContextHelper>();
    }

    public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<ObiletApiOptions>(config.GetSection("ObiletApi"));
        services.Configure<CookieSettings>(config.GetSection("CookieSettings"));
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[] { "tr-TR", "en-EN" }
                .Select(c => new CultureInfo(c))
                .ToList();

            options.DefaultRequestCulture = new RequestCulture("tr-TR");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            
            options.RequestCultureProviders =
            [
                new CookieRequestCultureProvider(), 
                new QueryStringRequestCultureProvider(),
                new AcceptLanguageHeaderRequestCultureProvider()
            ];
        });

        return services;
    }

    public static IServiceCollection AddCustomLocalization(this IServiceCollection services)
    {
        return services.AddLocalization(options => options.ResourcesPath = "Resources");
    }

    public static IServiceCollection AddCustomRedis(this IServiceCollection services, IConfiguration config)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = config.GetConnectionString("Redis");
            options.InstanceName = "JourneyFinder_";
        });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
        });

        return services;
    }
}
