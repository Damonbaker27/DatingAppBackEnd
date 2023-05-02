using API.Data;
using API.Helper;
using API.Interface;
using API.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration config)
        {

            //use sql server
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DatingAppConnectionString"));
            });

            //set the cors policy to allow any request
            services.AddCors(options => options.AddPolicy("DefaultCors", policy =>
                policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()));
            
            //allow user repository to be injected
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ILikesRepository, UserLikeRepository>();

            //add JWT token service
            services.AddScoped<ITokenService, TokenService>();

            //add mapping for entities to dtos
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            //add settings cloudinary config file to services container
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            
            //allow photoservice to be injectable.
            services.AddScoped<IPhotoService, PhotoService>();

            //add actionfilter
            services.AddScoped<LogUserActivity>();

            return services;
        }

       

    }
}
