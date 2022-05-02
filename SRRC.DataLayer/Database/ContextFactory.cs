using System.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
namespace SRRC.DataLayer.Database
{
    public sealed class ContextFactory : IDesignTimeDbContextFactory<SRRCDbContext>
    {
        public ContextFactory()
        {

        }
        public readonly IHttpContextAccessor _accessor;
        public ContextFactory(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public SRRCDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<SRRCDbContext>();
            var basePath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                                    .SetBasePath(basePath)
                                    .AddJsonFile("appsettings.json")
                                    .Build();

            builder.UseSqlite(configuration.GetConnectionString("SRRCConnectionString"));

            return new SRRCDbContext(builder.Options, _accessor);
        }
    }
}
