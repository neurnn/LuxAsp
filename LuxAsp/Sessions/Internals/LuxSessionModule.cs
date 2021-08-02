using LuxAsp.Sessions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace LuxAsp.Sessions.Internals
{
    [LuxHostModule(int.MinValue)]
    internal class LuxSessionModule : LuxHostModule
    {
        protected override void Configure(ILuxHostBuilder Builder)
        {
            Builder.Use<LuxSessionOptions>();
            Builder.ConfigureServices(Priority.Before(Priority.Session), App =>
            {
                var Options = Builder.GetConfiguration<LuxSessionOptions>();

                App.AddSingleton(Options);
                App.AddSingleton<ILuxSessionStoreWorker, LuxSessionStoreWorker>();

                /* Add the Session as Http Request Service.
                 * if the ILuxSession requested, then, opens a new session. */
                App.AddHttpCleanableService(Http =>
                {
                    var Store = Http.RequestServices.GetRequiredService<ILuxSessionStore>();

                    using var CTS1 = new CancellationTokenSource(Options.Timeout);
                    var Session = Store.OpenAsync(Http, CTS1.Token).Result;

                    return new Cleanable<ILuxSession>(Session).Configure(_Session =>
                        {
                            if (_Session != null)
                            {
                                using var CTS2 = new CancellationTokenSource(Options.Timeout);
                                try { Store.CloseAsync(_Session, false, CTS2.Token).Wait(); }
                                catch { }
                            }
                        });
                });
            });
        }
    }
}
