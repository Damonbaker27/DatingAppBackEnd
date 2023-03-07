using API.Data;
using API.Helper;
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

            services.AddCors(options => options.AddPolicy("DefaultCors", policy =>
                policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
            
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            //add settings cloudinary config file to services container
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            
            //allow photoservice to be injectable.
            services.AddScoped<IPhotoService, PhotoService>();

            return services;
        }

       

    }
}
