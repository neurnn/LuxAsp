using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuxAsp.Internals
{
    [LuxHostModule(int.MinValue)]
    internal class DatabaseModule : LuxHostModule
    {
        /// <summary>
        /// Migration completion's notifier.
        /// </summary>
        private class Migrations : IDatabaseMigrations, IDisposable
        {
            private TaskCompletionSource<bool> m_MigrationTCS = new TaskCompletionSource<bool>();

            /// <summary>
            /// Set completion of the migration process.
            /// </summary>
            public void OnCompleted() => m_MigrationTCS.TrySetResult(true);

            /// <summary>
            /// Wait Migration completed asynchronously.
            /// </summary>
            /// <param name="Cancellation"></param>
            /// <returns></returns>
            public async Task WaitMigrationAsync(CancellationToken Cancellation = default)
            {
                if (m_MigrationTCS.Task.IsCompleted)
                    return;

                var CancellationTCS = new TaskCompletionSource<bool>();
                using (Cancellation.Register(() => CancellationTCS.TrySetCanceled()))
                    await (await Task.WhenAny(m_MigrationTCS.Task, CancellationTCS.Task));
            }

            /// <summary>
            /// Cancel the migration completion when disposed.
            /// </summary>
            public void Dispose() => m_MigrationTCS.TrySetCanceled();
        }

        /// <summary>
        /// Configure the application.
        /// </summary>
        /// <param name="Builder"></param>
        protected override void Configure(ILuxHostBuilder Builder)
        {
            Builder.ConfigureServices(int.MaxValue,
                X => X.AddSingleton<IDatabaseMigrations, Migrations>());

            Builder.ConfigureMigrations(int.MinValue, Services =>
            {
                Services
                    .GetService<DatabaseAccess>()
                    .Instance.Database.Migrate();
            });

            Builder.ConfigureMigrations(int.MaxValue, Services =>
            {
                var Migrations = Services.GetRequiredService<IDatabaseMigrations>();
                if (Migrations is Migrations _Migration) _Migration.OnCompleted();
            });
        }
    }
}
