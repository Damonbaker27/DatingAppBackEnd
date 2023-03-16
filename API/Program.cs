using API.Data;
using API.Extensions;
using API.Interface;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Runtime;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddApplicationServices(builder.Configuration);

//add JWT authentication
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

//use the new middleware
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{

	app.UseSwagger();
	app.UseSwaggerUI();

}


//Middleware for building request pipeline.
app.UseHttpsRedirection();

//use CORS policy
app.UseCors("DefaultCors");

// checks for valid token.

app.UseAuthentication();

// looks at what user is allowed to do.
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();

var service = scope.ServiceProvider;

try
{
	//retrive data context and create database/apply all migrations
	var context = service.GetRequiredService<DataContext>();
	await context.Database.MigrateAsync();
	await Seed.SeedUsers(context);
}
catch (Exception ex)
{
	var logger = service.GetService<ILogger<Program>>();
	logger.LogError(ex, "An error occurred during migration");
}

app.Run();
