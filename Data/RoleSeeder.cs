using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SmartInventorySystem.Models;

namespace SmartInventorySystem.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = serviceProvider.GetRequiredService<ILogger<IdentityRole>>();

            string[] roles = { "Admin", "Staff" };

            logger.LogInformation("Starting role seeding");
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    logger.LogInformation("Creating role: {Role}", role);
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        logger.LogError("Failed to create role '{Role}': {Errors}", role, errors);
                        throw new Exception($"Failed to create role '{role}': {errors}");
                    }
                }
                else
                {
                    logger.LogInformation("Role already exists: {Role}", role);
                }
            }

            // Seed Admin and Staff Users
            logger.LogInformation("Starting user seeding");
            await CreateUserWithRole(userManager, logger, "admin@inventory.com", "Admin123!", "Admin", "Admin User");
            await CreateUserWithRole(userManager, logger, "staff@example.com", "Staff@123", "Staff", "Staff User");
            logger.LogInformation("User seeding completed");
        }

        private static async Task CreateUserWithRole(
            UserManager<ApplicationUser> userManager,
            ILogger<IdentityRole> logger,
            string email,
            string password,
            string role,
            string fullName)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                logger.LogInformation("Creating user with email: {Email} and role: {Role}", email, role);
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    FullName = fullName,
                    CreatedDate = DateTime.Now
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogError("Failed to create {Role} user: {Errors}", role, errors);
                    throw new Exception($"Failed to create {role} user: {errors}");
                }
            }
            else
            {
                logger.LogInformation("User already exists: {Email}", email);
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                logger.LogInformation("Adding user {Email} to role {Role}", email, role);
                await userManager.AddToRoleAsync(user, role);
            }
            else
            {
                logger.LogInformation("User {Email} is already in role {Role}", email, role);
            }
        }
    }
}