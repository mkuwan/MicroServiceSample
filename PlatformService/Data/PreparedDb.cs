using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PreparedDb
{
    public static void PrepPopulation(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();

        if (dbContext == null)
            return;

        if (app.Environment.IsProduction())
        {
            try
            {
                dbContext.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($">> Could not run migrations: {ex.Message}");
                return;
            }
        }
        SeedData(dbContext);
    }

    private static void SeedData(AppDbContext dbContext)
    {
        if (!dbContext.Platforms.Any())
        {
            Console.WriteLine(">> Seeding Data...");

            dbContext.Platforms.AddRange(
                new Platform() { Name = "Dot Net", Publisher = "Microsoft", Cost = "Free" },
                new Platform() { Name = "プログラミングC#", Publisher = "O'REILLY", Cost = "5280円" },
                new Platform() { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
                );

            dbContext.SaveChanges();
        }
        else
        {
            Console.WriteLine(">> すでに初期データがあります");
        }
    }
}