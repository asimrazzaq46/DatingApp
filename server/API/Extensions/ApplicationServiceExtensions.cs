using System;
using API.Data;
using API.Data.Repositery;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
                                                                     IConfiguration config)
    {
        // Add services to the container.

        services.AddControllers();

        // Adding Database into Services
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });

        // Adding TokenService into our service

        services.AddScoped<ITokenService, TokenService>();
        // Adding CORS into our services
        services.AddCors(options =>
             {
                 // this defines a CORS policy called "default"
                 options.AddPolicy("default", policy =>
                 {
                     options.AddPolicy("CorsPolicy",
                      builder => builder.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials()
                      .WithOrigins("http://localhost:4200", "https://localhost:4200")
                          );
                 });
             });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepositery, UserRepositery>();
        services.AddScoped<ILikesRepositery, LikesRepositery>();
        services.AddScoped<IMessageRepositery, MessageRepositery>();
        services.AddScoped<IPhotoRepositery, PhotoRepositery>();
        services.AddScoped<IPhotoService, PhotoService>();
        services.AddScoped<LogUserActivity>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
        services.AddSignalR();
        services.AddSingleton<PresenceTracker>();

        return services;

    }
}
