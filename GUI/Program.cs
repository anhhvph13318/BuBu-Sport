using GUI.Hubs;
using GUI.Shared.Common;
using GUI.Shared.VNPay;

var builder = WebApplication.CreateBuilder(args);

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<OrderHub>("/order-hub");

app.Run();