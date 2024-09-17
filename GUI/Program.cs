using GUI.Hubs;
using GUI.Shared.Common;
using GUI.Shared.VNPay;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Routing.Patterns;

var builder = WebApplication.CreateBuilder(args);

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

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
//builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.Configure<CommonSettings>(builder.Configuration.GetSection("CommonSettings"));
builder.Services.AddSession();
builder.Services.AddSignalR();

builder.Services.AddTransient<VNPayService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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