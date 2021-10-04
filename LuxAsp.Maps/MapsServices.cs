using LuxAsp.Maps;
using LuxAsp.Maps.Internals;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LuxAsp
{
    /// <summary>
    /// Map Service Configuration.
    /// </summary>
    public sealed class MapsServices : LuxHostConfiguration
    {
        /// <summary>
        /// Gets or Sets the Geoplace information.
        /// </summary>
        public Func<IServiceProvider, IMapsGeoplaceService> Geoplace { get; set; }

        /// <summary>
        /// Geoplace Type. (Func property has higher priority.
        /// </summary>
        public Type GeoplaceType { get; set; }

        /// <summary>
        /// Configures the neccessary services.
        /// </summary>
        /// <param name="Builder"></param>
        protected override void Configure(ILuxHostBuilder Builder)
        {
            Builder.ConfigureServices(AddGeoplaceSingletons);
        }

        /// <summary>
        /// Adds the singleton geoplace service.
        /// </summary>
        /// <param name="App"></param>
        private void AddGeoplaceSingletons(IServiceCollection App)
        {
            if (Geoplace is null && GeoplaceType is null)
                App.AddSingleton<IMapsGeoplaceService, MapsNullGeoplaceService>();

            else if (GeoplaceType != null)
                App.AddSingleton(typeof(IMapsGeoplaceService), GeoplaceType);

            else
                App.AddSingleton(Geoplace);
        }
    }
}
