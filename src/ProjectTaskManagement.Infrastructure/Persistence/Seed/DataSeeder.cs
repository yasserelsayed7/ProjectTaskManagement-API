using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectTaskManagement.Domain.Entities;
using ProjectTaskManagement.Domain.Enums;
using ProjectTaskManagement.Application.Common.Interfaces;

namespace ProjectTaskManagement.Infrastructure.Persistence.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
            return;

        logger.LogInformation("Seeding database...");

        var admin = new ApplicationUser
        {
            Email = "admin@projecttask.com",
            FullName = "System Admin",
            PasswordHash = passwordHasher.Hash("Admin@123"),
            Role = UserRoles.Admin
        };

        var user = new ApplicationUser
        {
            Email = "user@projecttask.com",
            FullName = "Demo User",
            PasswordHash = passwordHasher.Hash("User@123"),
            Role = UserRoles.User
        };

        context.Users.AddRange(admin, user);
        await context.SaveChangesAsync();

        var project = new Project
        {
            Name = "Sample E-Commerce Platform",
            Description = "Build a scalable online store with microservices.",
            UserId = user.Id
        };

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        context.Tasks.AddRange(
            new TaskItem
            {
                Title = "Design API contracts",
                Description = "Define OpenAPI specs for all services.",
                Priority = TaskPriority.High,
                Status = TaskItemStatus.InProgress,
                ProjectId = project.Id,
                DueDate = DateTime.UtcNow.AddDays(7)
            },
            new TaskItem
            {
                Title = "Setup CI/CD pipeline",
                Description = "Configure GitHub Actions for automated deployments.",
                Priority = TaskPriority.Medium,
                Status = TaskItemStatus.Todo,
                ProjectId = project.Id,
                DueDate = DateTime.UtcNow.AddDays(14)
            });

        await context.SaveChangesAsync();
        logger.LogInformation("Database seeding completed.");
    }
}
