using LuxAsp.Internals;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace LuxAsp
{
    /// <summary>
    /// Create Database instance.
    /// This is for providing the database instance to nuget efcore commands.
    /// </summary>
    /// <typeparam name="TApp"></typeparam>
    public abstract class DatabaseMigrator<TApp> : IDesignTimeDbContextFactory<Database>
        where TApp : LuxHostApplication, new ()
    {
        /// <summary>
        /// Creates a New DbContext.
        /// </summary>
        /// <param name="Args"></param>
        /// <returns></returns>
        public Database CreateDbContext(string[] Args)
        {
            var Builder = LuxHostApplication.DryRun<TApp>(Args);

            var Configs = Builder.GetConfiguration<DatabaseConfiguration>();
            if (Configs is null)
                throw new InvalidOperationException("The application has no database configuration.");

            var Configurations = new ConfigurationBuilder();
            if (Builder is ILuxDryCaptureSettings Capture)
                Capture.CaptureSettings(Configurations);

            return (Configs.GetOptions() as IDatabaseDryCreate)
                .DryCreate(Configurations.Build());
        }
    }
}
