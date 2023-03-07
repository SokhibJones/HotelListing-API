using HotelListing.Core;
using HotelListing.Data;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using HotelListing.Core.IRepository;
using HotelListing.Core.Repository;
using HotelListing.Core.Services;
using HotelListing.Core.Configurations;
using AspNetCoreRateLimit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseLazyLoadingProxies().UseSqlServer(connectionString);
});

builder.Services.AddMemoryCache();
builder.Services.ConfigureRateLimiting();
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureHttpCacheHeaders();

// Configuring authentication and identity services
//builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwt(builder.Configuration);

builder.Services.ConfigureVersioning();

builder.Services.AddAutoMapper(typeof(MapperInitializer));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthManager, AuthManager>();

builder.Services.AddControllers(options =>
{
    options.CacheProfiles.Add("2MinutesDuration", new Microsoft.AspNetCore.Mvc.CacheProfile
    {
        Duration = 120
    });
}).AddNewtonsoftJson(option => option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

// Add ApiExplorer to discover versions
builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerDoc();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// Serilog configuration
string logFilesPath = "LogFiles\\log-.txt";
builder.Host.UseSerilog((context, loggerConfiguration) =>
{
    loggerConfiguration
        .WriteTo.File(
                    path: logFilesPath,
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);
});

/* Adding CORS policy */
builder.Services.AddCors(option =>
{
    option.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
        }
    });
}

app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseCors(policyName: "AllowAll");

app.UseResponseCaching();
app.UseHttpCacheHeaders();
app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
