using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.IO;
using static LuxAsp.LuxHostModule;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace LuxAsp.EFCore.TestSuite
{
    /// <summary>
    /// Program class.
    /// </summary>
    public class Program : LuxHostApplication
    {
        public static void Main(string[] args) => Run<Program>(args);

        /// <summary>
        /// Configure the Host Application.
        /// </summary>
        /// <param name="Builder"></param>
        /// <param name="Arguments"></param>
        protected override void Configure(ILuxHostBuilder Builder, string[] Arguments)
        {
            Builder.ConfigureDatabase(Options =>
            {
                Options
                    .Setup((Configuration, DbOptions) =>
                    {
                        var Config = new MySqlConnectionStringBuilder();

                        Config.Server = Configuration.GetValue("MySQL:Server", "localhost");
                        Config.UserID = Configuration.GetValue("MySQL:User", "root");
                        Config.Password = Configuration.GetValue("MySQL:Password", "");
                        Config.Database = Configuration.GetValue("MySQL:Database", null as string);
                        Config.Port = Configuration.GetValue("MySQL:Port", 3306u);
                        Config.CharacterSet = Configuration.GetValue("MySQL:CharacterSet", "utf8mb4"); ;

                        DbOptions.UseMySQL(Config.ToString(),
                            X => X.MigrationsAssembly(typeof(Program).Assembly.GetName().Name));
                    })

                    .WithDocumentModel()
                    .WithUserModel()
                    .WithTokenModel();
            });

            Builder.Configure(Priority.Statics, (App, Env) =>
            {
                App.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(AppDir("/storage").FullName),
                    RequestPath = "/files"
                });
            });

            Builder.ConfigureStorage(Storages =>
            {
                /* Mount Storage Directory. */
                Storages.MountFileSystem(null, AppDir("/storage"), "/files");
            });
        }
    }
}
