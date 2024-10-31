using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WebCityEvents.Data;
using WebCityEvents.Models;

public class DbInitializationMiddleware
{
    private readonly RequestDelegate _next;

    public DbInitializationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await DbInitializer.InitializeAsync(userManager, roleManager);
        await _next(context);
    }
}
