using JourneyFinder.Extensions;
using JourneyFinder.Filters;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

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
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.UseForwardedHeaders();
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();