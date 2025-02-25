using AuthService.Interfaces;
using AuthService.Interfaces.desktopinterface;
using AuthService.Model;
using AuthService.Services;
using AuthService.Services.desktopservice;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace AuthService
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add DbContext (ApplicationDbContext) to connect with SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            // Add Authentication and Authorization with JWT Bearer Token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
                    };
                });

            // Add Swagger with Bearer Token Authorization setup
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            // Add other necessary services
            services.AddSingleton<TokenService>();
            services.AddScoped<IAuthService, AuthService.Services.AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<EmailService>();
            services.AddScoped<PhoneOTPService>();
            services.AddHttpClient<PhoneOTPService>();
            services.AddScoped<IMembershipUserService, MembershipUserService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IStripeService, StripeService>();
            services.AddScoped<IPriceService, PriceService>();
            services.AddScoped<IDesktopCourtSportsService, DesktopCourtSportsService>();
            services.AddScoped<IDesktopBookingService, DesktopBookingService>();


            // Add Controllers for API endpoints
            services.AddControllers();

            // Add Swagger API documentation support
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // Use Swagger UI for API documentation in Development mode
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Enable HTTPS Redirection
            app.UseHttpsRedirection();

            // Routing configuration
            app.UseRouting();

            // Use Authentication and Authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Map API Controllers
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}





////////////////////////
///
/*
using AuthService.Interfaces;
using AuthService.Model;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace AuthService
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add DbContext (ApplicationDbContext) to connect with SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(_configuration.GetConnectionString("DefaultConnection")));

            // Add Authentication and Authorization with JWT Bearer Token
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        ValidAudience = _configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
                    };
                });

            // Add Swagger with Bearer Token Authorization setup
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            // Add other necessary services
            services.AddSingleton<TokenService>();
            services.AddScoped<IAuthService, AuthService.Services.AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<EmailService>();
            services.AddScoped<PhoneOTPService>();
            services.AddHttpClient<PhoneOTPService>();
            services.AddScoped<IMembershipUserService, MembershipUserService>();
            services.AddScoped<IBookingService, BookingService>();

            // Add Controllers for API endpoints
            services.AddControllers();

            // Add Swagger API documentation support
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Ensure the application listens on 127.0.0.1:7070
            *//*var forwardHeadersOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.All
            };
            app.UseForwardedHeaders(forwardHeadersOptions);
*//*
            // Enable Swagger in all environments
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty; // Makes Swagger accessible at root URL `/`
            });

            // Enable HTTPS Redirection
            app.UseHttpsRedirection();

            // Routing configuration
            app.UseRouting();

            // Use Authentication and Authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            // Map API Controllers
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}*/
