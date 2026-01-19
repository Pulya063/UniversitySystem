using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using UniversitySystem.Models;

namespace UniversitySystem.Data
{
    // Design-time factory for EF tools (migrations)
    public class UniversityContextFactory : IDesignTimeDbContextFactory<UniversityContext>
    {
        public UniversityContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<UniversityContext>();
            var connStr = System.Environment.GetEnvironmentVariable("CONNECTION_STRING")
                          ?? "Host=localhost;Port=5432;Database=WirtualnyDziekanat;Username=postgres;Password=SuperStrongPassword123!;";
            UniversityContext.Configure(builder, connStr);
            return new UniversityContext(builder.Options);
        }
    }
}
