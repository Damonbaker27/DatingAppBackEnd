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
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

//use the new middleware

app.UseMiddleware<ExceptionMiddleware>();

//if (app.Environment.IsDevelopment())
//{

//    app.UseSwagger();
//    app.UseSwaggerUI();

//}


//Middleware for building request pipeline.
app.UseHttpsRedirection();

//use cors policy
app.UseCors("DefaultCors");

// checks for valid token.
app.UseAuthentication();

// looks at what user is allowed to do.
app.UseAuthorization();

app.MapControllers();

app.Run();
