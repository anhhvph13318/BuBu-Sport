using DATN_ACV_DEV;
using DATN_ACV_DEV.Controllers;
using DATN_ACV_DEV.Entity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Hỗ trợ API
builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer("Data Source=TRINH-KERIA\\KTRINH;Initial Catalog=DB_DraftBracnh_04_03;Integrated Security=True;Trust Server Certificate=True;Encrypt=False;"));

builder.Services.AddControllers();
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer("Data Source=DEC\\SQLEXPRESS;Initial Catalog=DB_BuBu_06_04;Integrated Security=True;Trust Server Certificate=True; Encrypt=False;"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped<IEmailService, EmailService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Sử dụng CORS
app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

app.MapControllers();

app.Run();