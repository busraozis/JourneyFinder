using System.Net;
using JourneyFinder.Factories;
using JourneyFinder.Filters;
using JourneyFinder.Helpers;
using JourneyFinder.Managers;
using JourneyFinder.Managers.Interfaces;
using JourneyFinder.Options;
using JourneyFinder.Services;
using JourneyFinder.Services.Interfaces;
using JourneyFinder.Settings;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IBusLocationService, BusLocationService>();
builder.Services.AddScoped<IJourneyService, JourneyService>();
builder.Services.AddScoped<ISessionManager, SessionManager>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();
builder.Services.AddScoped<IJourneyManager, JourneyManager>();
builder.Services.AddScoped<ICookieManager, CookieManager>();
builder.Services.AddScoped<IRequestContextHelper, RequestContextHelper>();
builder.Services.AddScoped<IDeviceRequestFactory, DeviceRequestFactory>();

builder.Services.AddScoped<JourneyValidationFilter>();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownProxies.Add(IPAddress.Parse("127.0.0.1")); 
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "JourneyFinder_"; 
});

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".JourneyFinder.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.IdleTimeout = TimeSpan.FromDays(30); 
});

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.Configure<ObiletApiOptions>(
    builder.Configuration.GetSection("ObiletApi"));
builder.Services.Configure<CookieSettings>(builder.Configuration.GetSection("CookieSettings"));

builder.Services.AddControllersWithViews(o =>
    {
        o.Filters.Add<PersistentCookieActionFilter>();
    })
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization();

var supportedCultures = new[] { "tr-TR", "en-EN" };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("tr-TR");
    options.SupportedCultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
});

var app = builder.Build();
app.UseRequestLocalization();
app.UseForwardedHeaders();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthorization();
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(locOptions.Value);
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();