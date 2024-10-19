using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
     {
         // this defines a CORS policy called "default"
         options.AddPolicy("default", policy =>
         {
             options.AddPolicy("CorsPolicy",
              builder => builder.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              );
         });
     });


builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});



var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors("CorsPolicy");
app.MapControllers();

app.Run();
