using Technical_Assessment_ElectroPi.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Technical_Assessment_ElectroPi.Infrastructure
{
    public class TechnicalAssessmentDbContextContextFactory : IDesignTimeDbContextFactory<TechnicalAssessmentDbContext>
    {
        public TechnicalAssessmentDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var optionsBuilder = new DbContextOptionsBuilder<TechnicalAssessmentDbContext>();
            var connectionString = configuration.GetConnectionString("Technical_Assessment_ElectroPiConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new TechnicalAssessmentDbContext(optionsBuilder.Options);
        }
    }
}
