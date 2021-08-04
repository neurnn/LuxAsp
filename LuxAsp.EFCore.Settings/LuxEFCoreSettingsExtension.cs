using LuxAsp.Internals;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuxAsp
{
    public static class LuxEFCoreSettingsExtension
    {
        /// <summary>
        /// Enable the Basic Settings Model and its Repository implementations.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static IDatabaseOptions WithBasicSettings(this IDatabaseOptions This)
            => This.With<Settings, SettingsRepository>();

        /// <summary>
        /// Use Settings Model with Basic Settings Repository.
        /// </summary>
        /// <param name="This"></param>
        /// <returns></returns>
        public static SettingsConfiguration WithBasicSettings<TModel>(this SettingsConfiguration This) where TModel : SettingsModel
            => This.With<TModel>(X => X.GetRequiredService<SettingsRepository>());
    }
}
