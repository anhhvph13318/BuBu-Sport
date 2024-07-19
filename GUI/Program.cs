using GUI.Shared.CommonSettings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddMvc();
builder.Services.Configure<CommonSettings>(builder.Configuration.GetSection("CommonSettings"));
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromSeconds(86400); // Khai báo khoảng thời gian để ession timeout 86400= 1 ngày
}); 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession(); // Sử dụng Session
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();