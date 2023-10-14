using Application.Activities;
using Application.Core;
using Application.Interfaces;
using Application.Interfaces.Strategies;
using Application.RuleEngine;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Photos;
using Infrastructure.Security;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<DataContext>(options =>
            {
                var connStr = config.GetConnectionString("DefaultConnection");

                options.UseNpgsql(connStr);
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithOrigins("http://localhost:3000")
                        .AllowCredentials();
                });
            });
            services.AddMediatR(typeof(List.Handler));
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Create>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUserAccessor, UserAccessor>();
            services.Configure<CloudinarySettings>(config.GetSection("Cloudinary"));
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            services.AddScoped<IRuleEngine, RuleEngine>();
            services.AddScoped<IEngineFunctions, EngineFunctions>();
            services.AddScoped<IActionStrategy, SetActionStrategy>();
            services.AddScoped<IActionStrategy, AppendActionStrategy>();
            services.AddScoped<IActionStrategy, PrependActionStrategy>();
            services.AddScoped<IActionStrategy, AddActionStrategy>();
            services.AddScoped<IActionStrategy, SubtractActionStrategy>();
            services.AddScoped<IActionStrategy, MultiplyActionStrategy>();
            services.AddScoped<IActionStrategy, DivideActionStrategy>();
            services.AddScoped<IActionStrategy, ExpressionActionStrategy>();
            services.AddScoped<ActionStrategyFactory>();
            services.AddSignalR();
            services.AddControllers().AddNewtonsoftJson();

            return services;
        }
    }
}