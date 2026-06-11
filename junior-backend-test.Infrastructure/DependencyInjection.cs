using junior_backend_test.Domain.Interfaces;
using junior_backend_test.Domain.Model;
using junior_backend_test.Infrastructure.Database;
using junior_backend_test.Infrastructure.Repositories;
using junior_backend_test.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using unior_backend_test.Domain.Interfaces;

namespace junior_backend_test.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IAccountManager, AccountManager>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddHostedService<UnverifiedUserCleanupService>();

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<JuniorBackendTestContext>()
                .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();

            var securityKeyString = configuration["Jwt:SecurityKey"];
            var securityKeyByte = Encoding.ASCII.GetBytes(securityKeyString);
            SecurityKey securityKey = new SymmetricSecurityKey(securityKeyByte);

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "mahmoud";
                option.DefaultChallengeScheme = "mahmoud";
            })
            .AddJwtBearer("mahmoud", builder =>
            {
                builder.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey
                };
            });

            return services;

        }
    }
}
