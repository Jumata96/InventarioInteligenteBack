using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InventarioInteligenteBack.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Cargar .env en tiempo de diseño
            Env.Load();

            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables() // lee CONNECTIONSTRINGS__DEFAULT
                .Build();

            var conn = config.GetConnectionString("Default");
            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException("ConnectionStrings:Default no configurada (ver .env).");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(conn);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
