using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp.Internals
{
    [LuxHostModule(int.MinValue)]
    internal class DatabaseModule : LuxHostModule
    {
        protected override void Configure(ILuxHostBuilder Builder)
        {
            Builder.ConfigureMigrations(int.MinValue, Services =>
            {
                Services
                    .GetService<DatabaseAccess>()
                    .Instance.Database.Migrate();
            });
        }
    }
}
