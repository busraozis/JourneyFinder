using AspNetCoreRateLimit;
using HealthChecks.UI.Client;
using JourneyFinder.Extensions;
using JourneyFinder.Filters;
using JourneyFinder.Options;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

if (args.Contains("--docker"))
{
    builder.WebHost.UseUrls("http://0.0.0.0:80");
}

builder.Services
    .AddHttpClient()
    .AddCustomServices()
    .AddCustomManagers()
    .AddCustomBuilders()
    .AddCustomFactories()
    .AddCustomHelpers()
    .AddCustomFilters()
    .AddCustomConfiguration(builder.Configuration)
    .AddCustomLocalization()
    .AddCustomRedis(builder.Configuration)
    .AddSession(options =>
    {
        options.Cookie.Name = ".JourneyFinder.Session";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.IdleTimeout = TimeSpan.FromDays(30);
    });

builder.Services.AddMemoryCache();

builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

builder.Services.AddHealthChecks()
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? string.Empty, name: "redis", failureStatus: HealthStatus.Degraded);

builder.Services.Configure<ObiletApiOptions>(options =>
{
    builder.Configuration.GetSection("ObiletApi").Bind(options);
    options.ApiKey = builder.Configuration["ObiletApiKey"] ?? string.Empty; 
});

builder.Services.AddControllersWithViews(options =>
    {
        options.Filters.Add<PersistentCookieActionFilter>();
    })
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);
app.UseHttpsRedirection();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    ForwardLimit = 1
});

app.UseRouting();
app.UseSession();
app.UseAuthorization();
app.UseIpRateLimiting();
app.UseForwardedHeaders();
app.MapStaticAssets();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();