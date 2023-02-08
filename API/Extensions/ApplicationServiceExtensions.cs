using API.Data;
using API.Interface;
using API.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration config)
        {

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DatingAppConnectionString"));
            });

            services.AddScoped<ITokenService, TokenService>();

            services.AddCors(options => options.AddPolicy("DefaultCors", policy =>
                policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));


            return services;
        }

       

    }
}
