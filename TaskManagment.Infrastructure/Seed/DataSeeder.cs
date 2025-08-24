using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagment.Domain.Entities;
using TaskManagment.Domain.Enums;
using TaskManagment.Domain.Identity;
using TaskManagment.Infrastructure.Data;

namespace TaskManagment.Infrastructure.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await ctx.Database.EnsureCreatedAsync();
            foreach (var role in new[] { "Admin", "User" })
            {
                if (!await roleMgr.RoleExistsAsync(role)) await roleMgr.CreateAsync(new IdentityRole(role));
            }
            var admin = await userMgr.FindByEmailAsync("admin@demo.com");
            if (admin is null)
            {
                admin = new AppUser { Email = "admin@demo.com", UserName = "admin@demo.com", FullName = "Admin Demo" };
                await userMgr.CreateAsync(admin, "Pass123$"); await userMgr.AddToRoleAsync(admin, "Admin");
            }
            var user = await userMgr.FindByEmailAsync("user@demo.com");
            if (user is null)
            {
                user = new AppUser { Email = "user@demo.com", UserName = "user@demo.com", FullName = "User Demo" };
                await userMgr.CreateAsync(user, "Pass123$"); await userMgr.AddToRoleAsync(user, "User");
            }
            if (!await ctx.Tasks.AnyAsync())
            {
                await ctx.Tasks.AddRangeAsync(new TaskItems { Title = "Set up project", Description = "Create initial repo", Status = TaskStatuses.New, AssignedUserId = admin.Id },
                new TaskItems { Title = "Build API", Description = "Implement endpoints", Status = TaskStatuses.InProgress, AssignedUserId = admin.Id },
                new TaskItems { Title = "Write docs", Description = "README & Swagger", Status = TaskStatuses.Done, AssignedUserId = user.Id });
                await ctx.SaveChangesAsync();
            }
        }
    }
}
