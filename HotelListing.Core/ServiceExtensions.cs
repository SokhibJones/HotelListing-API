using HotelListing.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Serilog;
using Marvin.Cache.Headers;
using AspNetCoreRateLimit;

namespace HotelListing.Core
{
    public static class ServiceExtensions
    {
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentityCore<ApiUser>(policy =>
            {
                policy.User.RequireUniqueEmail = true;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
            builder.AddTokenProvider("HotelListingApi", typeof(DataProtectorTokenProvider<ApiUser>));
            builder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = Environment.GetEnvironmentVariable("KEY");
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                    ValidAudience = jwtSettings.GetSection("Audience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });
        }

        public static void AddSwaggerDoc(this IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme
                    Enter 'Bearer' [space] and then your token in the text input field below",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
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
                            },
                            Scheme = "0auth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

                //option.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
            });
        }

        public static void ConfigureExceptionHandler(this IApplicationBuilder builder)
        {
            builder.UseExceptionHandler(options =>
            {
                options.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature is not null)
                    {
                        throw contextFeature.Error;
                        Log.Error($"Something went wrong: {contextFeature.Error}");
                        var error = new Error
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = "Internal Server Error. Please try later."
                        };
                        await context.Response.WriteAsync(error.ToString());
                    }
                });
            });
        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
        }

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            services.AddResponseCaching();
            services.AddHttpCacheHeaders(
            expirationOptions =>
            {
                expirationOptions.MaxAge = 120;
                expirationOptions.CacheLocation = CacheLocation.Private;
            },
            validationOptions =>
            {
                validationOptions.MustRevalidate = true;
            });
        }

        public static void ConfigureRateLimiting(this IServiceCollection services)
        {
            var rateLimitingRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Limit = 1,
                    PeriodTimespan = TimeSpan.FromSeconds(5)
                }
            };
            services.Configure<IpRateLimitOptions>(options =>
            {
                options.GeneralRules = rateLimitingRules;
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        }
    }
}
