using junior_backend_test.Domain.Model;
using junior_backend_test.Infrastructure.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace junior_backend_test.Infrastructure.Services
{
    public class UnverifiedUserCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UnverifiedUserCleanupService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(2);
        private readonly TimeSpan _expirationTime = TimeSpan.FromMinutes(10);

        public UnverifiedUserCleanupService(IServiceProvider serviceProvider, ILogger<UnverifiedUserCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupUnverifiedUsersAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing unverified user cleanup.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CleanupUnverifiedUsersAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = scope.ServiceProvider.GetRequiredService<JuniorBackendTestContext>();

            var expirationThreshold = DateTime.UtcNow.Subtract(_expirationTime);

            var unverifiedUsers = await userManager.Users
                .Where(u => !u.EmailConfirmed && u.CreateAt < expirationThreshold)
                .ToListAsync();

            if (unverifiedUsers.Any())
            {
                foreach (var user in unverifiedUsers)
                {
                    var otps = await context.EmailOtps.Where(o => o.Email == user.Email).ToListAsync();
                    if (otps.Any())
                    {
                        context.EmailOtps.RemoveRange(otps);
                        await context.SaveChangesAsync();
                    }

                    var roles = await userManager.GetRolesAsync(user);
                    if (roles.Any())
                    {
                        await userManager.RemoveFromRolesAsync(user, roles);
                    }

                    var claims = await userManager.GetClaimsAsync(user);
                    if (claims.Any())
                    {
                        await userManager.RemoveClaimsAsync(user, claims);
                    }

                    var result = await userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Deleted unverified user {user.Email} due to inactivity.");
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to delete unverified user {user.Email}. Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }
    }
}
