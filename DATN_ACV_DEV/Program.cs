using DATN_ACV_DEV;
using DATN_ACV_DEV.Entity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer("Data Source=TW4NENH\\TUANANH;Initial Catalog=DB_BuBu_04_04;Integrated Security=True;Trust Server Certificate=True; Encrypt=False;"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(options =>
            options.WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyHeader());
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
