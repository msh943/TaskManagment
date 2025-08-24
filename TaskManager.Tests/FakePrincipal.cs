using System.Security.Claims;

namespace TaskManager.Tests
{
    public static class FakePrincipal
    {
        public static ClaimsPrincipal User(string userId, string role = "User", string? email = null)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Name, email ?? $"{userId}@demo.local"),
            new Claim(ClaimTypes.Email, email ?? $"{userId}@demo.local"),
        }, "TestAuth"));
        }

        public static ClaimsPrincipal Admin(string userId = "admin-id", string? email = "admin@demo.local")
            => User(userId, "Admin", email);
    }
}
