using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuxAsp.LuxHostModule;

namespace LuxAsp.Internals
{
    internal class WebSocketConfiguration : LuxHostConfiguration
    {
        protected override void Configure(ILuxHostBuilder Builder)
        {
            Builder.ConfigureServices(Priority.Before(Priority.EarlyDefault), Services =>
            {
                Services.AddSingleton<LuxWebSocketManager, LuxWebSocketManager>();
            });

            Builder.Configure(Priority.Before(Priority.EarlyDefault), (App, Env) =>
            {
                App.UseWebSockets(new WebSocketOptions
                {
                    KeepAliveInterval = TimeSpan.FromSeconds(60)
                });
            });
        }
    }
}
