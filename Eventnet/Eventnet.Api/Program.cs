using System.Text;
using Eventnet.Config;
using Eventnet.DataAccess;
using Eventnet.Domain;
using Eventnet.Helpers.EventFilterFactories;
using Eventnet.Infrastructure;
using Eventnet.Models.Authentication.Tokens;
using Eventnet.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtTokenConfig = builder.Configuration.GetSection("JWT").Get<JwtTokenConfig>();
var emailConfig = builder.Configuration.GetSection("Email").Get<EmailConfiguration>();

services.AddSingleton(emailConfig);
services.AddSingleton(jwtTokenConfig);
services.AddSingleton<IJwtAuthService, JwtAuthService>();
services.AddScoped<CurrentUserService>();

services.AddSingleton<IEventFilterService, EventFilterService>();
services.AddSingleton<IEventFilterFactory, LocationFilterFactory>();
services.AddSingleton<IEventFilterFactory, StartDateFilterFactory>();
services.AddSingleton<IEventFilterFactory, EndDateFilterFactory>();
services.AddSingleton<IEventFilterFactory, OwnerFilterFactory>();

services.AddScoped<IEmailService, EmailService>();

services.AddHttpContextAccessor();

services.AddControllers();

services.AddEndpointsApiExplorer();

services.AddDbContext<ApplicationDbContext>(
    opt => opt.UseNpgsql(connectionString));

services.AddAutoMapper(opt => opt.AddProfile<ApplicationMappingProfile>());

services.AddIdentity<UserEntity, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = jwtTokenConfig.Audience,
            ValidIssuer = jwtTokenConfig.Issuer,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenConfig.Secret))
        };
    });

services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

// ReSharper disable once UnusedType.Global Use for integration tests
// https://docs.microsoft.com/ru-ru/aspnet/core/test/integration-tests?view=aspnetcore-6.0#basic-tests-with-the-default-webapplicationfactory
namespace Eventnet
{
    public class Program
    {
    }
}