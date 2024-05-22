using EasyArchitect.Infrastructure.Cache;
using EasyArchitect.PageModel.AuthExtensions.PageFilters;
using Microsoft.AspNetCore.Authentication.Cookies;
using SalesCar.Application;
//using WebPagesLoginLab1.PageFilters;

var builder = WebApplication.CreateBuilder(args);

// 註冊 AppSettings Configuration 類型，可在類別中注入 IOptions<AppSettings>
IConfigurationSection appSettingRoot = builder.Configuration.GetSection("AppSettings");

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddScoped<IRedisCacheProvider, RedisCacheProvider>();

#pragma warning disable ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'
IServiceProvider serviceProvider = builder.Services.BuildServiceProvider();
#pragma warning restore ASP0000 // Do not call 'IServiceCollection.BuildServiceProvider' in 'ConfigureServices'

builder.Services.AddRazorPages(options =>
{
    // 僅對特定頁面（例如 Index 頁面）添加自定義過濾器
    options.Conventions.AddPageApplicationModelConvention("/Index", pageApplicationModel =>
    {
        pageApplicationModel.Filters.Add(new CustomAuthorizationFilter(appSettingRoot));
        //pageApplicationModel.Filters.Add(new CustomAuthorizationFilter(serviceProvider.GetRequiredService<IRedisCacheProvider>()));
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(configure =>
{
    configure.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    configure.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(appSettingRoot.GetValue<int>("TimeoutMinutes"));
    options.Cookie.HttpOnly = true;
    options.Events = new CookieAuthenticationEvents()
    {
        OnRedirectToReturnUrl = async (context) =>
        {
            context.HttpContext.Response.Cookies.Delete(Account.LOGIN_USER_INFO);
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
