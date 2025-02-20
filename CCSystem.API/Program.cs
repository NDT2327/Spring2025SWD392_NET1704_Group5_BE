﻿using CCSystem.API.Constants;
using CCSystem.API.Extentions;
using CCSystem.API.Middlewares;
using CCSystem.BLL.DTOs.JWTs;
using CCSystem.BLL.Service;
using CCSystem.BLL.Services.Implementations;
using CCSystem.BLL.Services.Interfaces;
using CCSystem.DAL.DBContext;
using CCSystem.DAL.Infrastructures;
using CCSystem.DAL.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddDbContext<SP25_SWD392_CozyCareContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDbStore"));
});
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbFactory, DbFactory>();
builder.Services.AddScoped<AccountRepository>();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opts => opts.SuppressModelStateInvalidFilter = true)
    .AddJsonOptions(options =>
    {
        // Chuyển đổi enum thành chuỗi khi trả về JSON
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddConfigSwagger();
//JWT
builder.AddJwtValidation();
//DI
builder.Services.Configure<JWTAuth>(builder.Configuration.GetSection("JWTAuth"));
builder.Services.AddDbFactory();
builder.Services.AddUnitOfWork();
builder.Services.AddServices();
builder.Services.AddScoped<CategoryRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddExceptionMiddleware();
//Middlewares
builder.Services.AddTransient<ExceptionMiddleware>();

//add CORS
builder.Services.AddCors(cors => cors.AddPolicy(
                            name: CorsConstant.PolicyName,
                            policy =>
                            {
                                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                            }
                        ));
//Middlewares
var app = builder.Build();
app.AddApplicationConfig();
app.Run();

