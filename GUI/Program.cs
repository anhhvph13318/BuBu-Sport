using DATN_ACV_DEV.Entity;
using GUI;
using GUI.Hubs;
using GUI.Shared.Common;
using GUI.Shared.VNPay;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Thêm các dịch vụ vào container trước khi Build()
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
        options.LoginPath = "/SignIn";
    });

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Thêm dịch vụ cho controllers và views
builder.Services.AddControllersWithViews();

// Thêm IHttpClientFactory để gọi API
builder.Services.AddHttpClient();

// Cấu hình CommonSettings
builder.Services.Configure<CommonSettings>(builder.Configuration.GetSection("CommonSettings"));

// Thêm các dịch vụ khác
builder.Services.AddSession();
builder.Services.AddSignalR();
builder.Services.AddScoped<DBContext>();
builder.Services.AddTransient<VNPayService>();
builder.Services.AddScoped<UserSession>();

// Xây dựng ứng dụng
var app = builder.Build();

// Cấu hình pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToFile("/forbidden", "forbidden.html");
app.MapHub<OrderHub>("/order-hub");

app.Run();