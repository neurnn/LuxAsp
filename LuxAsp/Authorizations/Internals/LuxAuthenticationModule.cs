using Microsoft.Extensions.DependencyInjection;

namespace LuxAsp.Authorizations.Internals
{
    [LuxHostModule(int.MinValue)]
    internal class LuxAuthenticationModule : LuxHostModule
    {
        protected override void Configure(ILuxHostBuilder Builder)
        {
            ConfigureAuthentication(Builder);
        }

        /// <summary>
        /// Configure the Authentication.
        /// </summary>
        /// <param name="Builder"></param>
        private void ConfigureAuthentication(ILuxHostBuilder Builder)
        {
            Builder.Use<LuxAuthenticationBuilder>();
            Builder.ConfigureServices(Priority.Authentication,
                App =>
                {
                    var AuthBuilder = Builder.GetConfiguration<LuxAuthenticationBuilder>();

                    /*
                     * This registers three services:
                     * 
                     * 1. ILuxAuthenticationProvider
                     * 2. ILuxAuthenticationTokenProvider
                     * 3. ILuxAuthenticationListener
                     */
                    App.AddHttpService(Context => AuthBuilder.BuildProvider(Context.Request));
                    App.AddHttpService(Context => AuthBuilder.BuildTokenProvider(Context.Request));
                    App.AddHttpService(Context => AuthBuilder.BuildListener(Context.Request));

                    /* Register ILuxAuthentication instance. */
                    App.AddHttpService<ILuxAuthentication>(HttpContext =>
                    {
                        return new LuxAuthentication(HttpContext,
                            HttpContext.GetService<ILuxAuthenticationProvider>(),
                            HttpContext.GetService<ILuxAuthenticationTokenProvider>(),
                            HttpContext.GetService<ILuxAuthenticationListener>());
                    });
                });

        }
    }
}
