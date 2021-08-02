using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LuxAsp.Internals
{
    internal interface IDatabaseDryCreate
    {
        /// <summary>
        /// Create an instance of the database.
        /// </summary>
        /// <returns></returns>
        Database DryCreate(IConfiguration Configuration);
    }
}
