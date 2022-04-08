using System.Text;
using Eventnet.Config;
using Eventnet.DataAccess;
using Eventnet.Helpers.EventFilterFactories;
using Eventnet.Infrastructure;
using Eventnet.Infrastructure.PhotoServices;
using Eventnet.Infrastructure.Validators;
using Eventnet.Models;
using Eventnet.Services;
using Eventnet.Services.SaveServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var jwtTokenConfig = builder.Configuration.GetSection("JWT").Get<JwtTokenConfig>();
var rabbitMqConfig = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqConfig>();
var photoStorageConfig = builder.Configuration.GetSection("PhotoStorage").Get<PhotoStorageConfig>();

services.AddSingleton(jwtTokenConfig);
services.AddSingleton(rabbitMqConfig);
services.AddSingleton(photoStorageConfig);
services.AddSingleton<IJwtAuthService, JwtAuthService>();

services.AddSingleton<IEventFilterFactory, LocationFilterFactory>();
services.AddSingleton<IEventFilterFactory, StartDateFilterFactory>();
services.AddSingleton<IEventFilterFactory, EndDateFilterFactory>();
services.AddSingleton<IEventFilterFactory, OwnerFilterFactory>();
services.AddSingleton<IEventFilterMapper, EventFilterMapper>();
services.AddSingleton<IPublishEventService, PublishEventService>();
services.AddSingleton<Handler>();
services.AddMemoryCache();
services.AddSingleton<IConsumeEventService, RabbitMqConsumeEventService>();
services.AddSingleton<IEventSaveService, EventSaveService>();
services.AddSingleton<IPhotoValidator, PhotoValidator>();
services.AddSingleton<IPhotoToStorageSaveService, PhotoToStorageSaveService>();
services.AddSingleton<ISaveToDbService, SaveToDbService>();
services.AddSingleton<IEventValidator, EventValidator>();
services.AddSingleton<IPhotoValidator, PhotoValidator>();
services.AddSingleton<IEventCreationValidator, EventCreationValidator>();
services.AddSingleton<IRabbitMqMessageHandler, RabbitMqMessageHandler>();

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

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

services.AddHostedService<BackgroundConsumeEventService>();

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
public partial class Program
{
}