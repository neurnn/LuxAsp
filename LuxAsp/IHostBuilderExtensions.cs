using LuxAsp.Internals;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;

namespace LuxAsp
{
    public static class IHostBuilderExtensions
    {
        private static object EXTENSION_KEY = new object();

        /// <summary>
        /// Configure Host with Lux ASP.NET Core framework.
        /// </summary>
        /// <param name="This"></param>
        public static IHostBuilder UseLuxAspCore(this IHostBuilder This, Action<ILuxHostBuilder> Callback)
        {
            var Props = This.Properties;
            var Instance = null as ILuxHostBuilder;

            lock (Props)
            {
                if (!Props.TryGetValue(EXTENSION_KEY, out var _Instance))
                     Props[EXTENSION_KEY] = (Instance = new LuxHostBuilder(This));

                else Instance = _Instance as ILuxHostBuilder;
            }

            Callback?.Invoke(Instance);
            return This;
        }
    }
}
